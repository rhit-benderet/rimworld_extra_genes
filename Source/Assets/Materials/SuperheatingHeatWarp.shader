Shader "SuperheatingHeatWarp"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _ColorHeatAccent ("ColorHeatAccent", Color) = (1,1,1,1)
        _Intensity ("Intensity", Range(-1,1)) = 0.1
        _XWidthFactor ("XWidthFactor", Float) = 1
        _MaxHeatAccent ("MaxHeatAccent", Float) = 0.2
        _HeatAccentFalloff ("HeatAccentFalloff", Float) = 0.5
        _HeatAccentVolatility ("HeatAccentVolatility", Float) = 5
        _RingMaxDistance ("RingMaxDistance", Float) = 0.2
        _AgeSecs ("AgeSecs", Float) = 0
        _RingCount ("RingCount", Float) = 4
        _PulseTime ("PulseTime", Float) = 4
    }
    SubShader
    {
        Tags
        {
            "QUEUE" = "Transparent-100"
            "RenderType"="Transparent"
        }
        LOD 100

        GrabPass
        {
        }

        Pass
        {
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "UnityCG.cginc"

            struct vertexInput
            {
                float4 vertex : POSITION;
                float2 tex_pos : TEXCOORD0;
            };

            struct vertexOutput
            {
                float2 uv : TEXCOORD0;
                float4 screen_pos : TEXCOORD1;
                float4 center_screen_pos : TEXCOORD2;
                float quad_width_pixels : TEXCOORD3;
                float quad_height_pixels : TEXCOORD4;
                float4 clip_pos : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;
            float4 _ColorHeatAccent;
            float _Intensity;
            float _XWidthFactor;
            float _AgeSecs;
            float _MaxHeatAccent;
            float _HeatAccentFalloff;
            float _HeatAccentVolatility;
            float _RingMaxDistance;
            float _PulseTime;
            float _RingCount;
            sampler2D _GrabTexture;
            static const float PI = 3.14159265f;

            vertexOutput vert(vertexInput input)
            {
                vertexOutput output;
                output.clip_pos = UnityObjectToClipPos(input.vertex);
                output.uv = TRANSFORM_TEX(input.tex_pos, _MainTex);
                output.screen_pos = ComputeGrabScreenPos(output.clip_pos);

                //object-space center of the quad/mesh, I believe
                float4 center_clip = UnityObjectToClipPos(float4(0, 0, 0, 1));
                output.center_screen_pos = ComputeGrabScreenPos(center_clip);
                float4 left_clip  = UnityObjectToClipPos(float4(-0.5, 0, 0, 1));
                float4 right_clip = UnityObjectToClipPos(float4( 0.5, 0, 0, 1));

                float2 left_screen  = (left_clip.xy / left_clip.w) * 0.5 + 0.5;
                float2 right_screen = (right_clip.xy / right_clip.w) * 0.5 + 0.5;
                
                output.quad_width_pixels = distance(
                    left_screen,
                    right_screen
                );
                
                float4 bottom_clip = UnityObjectToClipPos(float4(0, 0, -0.5, 1));
                float4 top_clip    = UnityObjectToClipPos(float4(0, 0, 0.5, 1));

                float2 bottom_screen = (bottom_clip.xy / bottom_clip.w) * 0.5 + 0.5;
                float2 top_screen    = (top_clip.xy / top_clip.w) * 0.5 + 0.5;

                output.quad_height_pixels = distance(
                    bottom_screen,
                    top_screen
                );
                
                return output;
            }

            fixed4 frag(vertexOutput input) : SV_Target
            {
                fixed4 tex_color = tex2D(_MainTex, input.uv);
                float mask_value = tex_color.r * tex_color.a;
                
                float2 screen_uv = input.screen_pos.xy / input.screen_pos.w;
                
                float2 center_screen_uv = input.center_screen_pos.xy / input.center_screen_pos.w;
                float2 unnorm_in_unscaled = center_screen_uv - screen_uv;
                unnorm_in_unscaled.x = unnorm_in_unscaled.x / input.quad_width_pixels;
                unnorm_in_unscaled.y = unnorm_in_unscaled.y / input.quad_height_pixels;
                float2 unnorm_in = unnorm_in_unscaled;
                unnorm_in.x = unnorm_in.x / _XWidthFactor;
                float2 norm_in = normalize(unnorm_in);
                norm_in.x = norm_in.x * _XWidthFactor;
                float theta = asin(norm_in.y);
                float t = fmod(_AgeSecs, 2 * PI);
                float r = (sin(2 * theta + t)/15 + sin(3 * theta + 4 * t)/4 + sin(7 * theta + t)/3 + sin(11 * theta + 3 * t)/2) / 3;
                
                float alpha = 1.0;
                for (int i = 0; i < _RingCount; i++)
                {
                    float t_before = _AgeSecs - i * _PulseTime / _RingCount;
                    if (t_before >= 0)
                    {
                        float t_ring = fmod(t_before, _PulseTime);
                        float d = (_RingMaxDistance + (r * _Intensity * _HeatAccentVolatility)) * t_ring / _PulseTime - (2 * length(unnorm_in_unscaled));
                        float length_factor = max(1.0- (_HeatAccentFalloff * (d * d)), 0);
                        float true_alpha = _MaxHeatAccent * _Color.a * length_factor * (1 - t_ring / _PulseTime);
                        alpha = alpha * (1-true_alpha);
                    }
                }
                
                
                float2 inward = norm_in;
                inward.x = inward.x * input.quad_width_pixels;
                inward.y = inward.y * input.quad_height_pixels;
                float2 offset = inward * r * _Intensity * mask_value * _Color.a;
                
                
                fixed4 bg_color = tex2D(_GrabTexture, screen_uv + offset);
                float3 saturated_color = _ColorHeatAccent.rgb;
                return float4(lerp(bg_color, saturated_color, 1 - alpha), 1);
            }
            ENDHLSL
        }
    }
}