Shader "Custom/laserbeamheatwarp" {
    Properties {
        _MainTex ("Main texture", 2D) = "white" {}
        _OpacityTex ("Opacity texture", 2D) = "white" {}
        _DistortionTex ("Distortion texture", 2D) = "white" {}
        _Color ("Color", Color) = (1, 1, 1, 1)
        _AgeSecs ("AgeSecs", Float) = 0
        _ChargeSpeed ("ChargeSpeed", Float) = 1.4
        _DistortionScrollSpeed ("DistortionScrollSpeed", Float) = 0.45
        _DistortionIntensity ("DistortionIntensity", Float) = 0.15
        _Delay ("Delay", Float) = 0.15
    }
    SubShader {
        Tags {
            "DisableBatching"="true"
            "IGNOREPROJECTOR"="true"
            "QUEUE"="Transparent-100"
            "RenderType"="Transparent"
        }
        Pass {
            Name ""
            Blend SrcAlpha OneMinusSrcAlpha, SrcAlpha OneMinusSrcAlpha
            ZClip On
            ZWrite Off
            Tags {
                "DisableBatching"="true"
                "IGNOREPROJECTOR"="true"
                "QUEUE"="Transparent-100"
                "RenderType"="Transparent"
            }
            CGPROGRAM
            
            #pragma vertex vert
            #pragma fragment frag

            

            struct appdata
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float2 texcoord : TEXCOORD;
            };
            struct v2f
            {
                float4 position : SV_POSITION;
                float2 texcoord : TEXCOORD;
                float texcoord1 : TEXCOORD1;
            };

            // CBs for DX11VertexSM40
            // Textures for DX11VertexSM40

            v2f vert(appdata v)
            {
                v2f o;
                float4 tmp0;
                float4 tmp1;
                tmp0 = v.vertex.yyyy * unity_ObjectToWorld._m01_m11_m21_m31;
                tmp0 = unity_ObjectToWorld._m00_m10_m20_m30 * v.vertex.xxxx + tmp0;
                tmp0 = unity_ObjectToWorld._m02_m12_m22_m32 * v.vertex.zzzz + tmp0;
                tmp0 = tmp0 + unity_ObjectToWorld._m03_m13_m23_m33;
                tmp1 = tmp0.yyyy * unity_MatrixVP._m01_m11_m21_m31;
                tmp1 = unity_MatrixVP._m00_m10_m20_m30 * tmp0.xxxx + tmp1;
                tmp1 = unity_MatrixVP._m02_m12_m22_m32 * tmp0.zzzz + tmp1;
                o.position = unity_MatrixVP._m03_m13_m23_m33 * tmp0.wwww + tmp1;
                tmp0.x = dot(unity_ObjectToWorld._m02_m12_m22, unity_ObjectToWorld._m02_m12_m22);
                o.texcoord1.x = sqrt(tmp0.x);
                o.texcoord.xy = v.texcoord.xy;
                return o;
            }

            struct fout
            {
                float4 sv_target : SV_Target;
            };

            // CBs for DX11PixelSM40
            float4 _Color; // 0 (starting at cb0[0].x)
            float _AgeSecs; // 16 (starting at cb0[1].x)
            float _ChargeSpeed; // 20 (starting at cb0[1].y)
            float _DistortionScrollSpeed; // 24 (starting at cb0[1].z)
            float _DistortionIntensity; // 28 (starting at cb0[1].w)
            float _Delay; // 32 (starting at cb0[2].x)
            // Textures for DX11PixelSM40
            sampler2D _DistortionTex; // 0
            sampler2D _MainTex; // 1

            fout frag(v2f inp)
            {
                fout o;
                float4 tmp0;
                float4 tmp1;
                tmp0.x = _AgeSecs < _Delay;
                if (tmp0.x) {
                    discard;
                }
                tmp0.x = _DistortionScrollSpeed * _AgeSecs;
                tmp0.x = frac(tmp0.x);
                tmp0.y = tmp0.x - 0.5;
                tmp0.xz = float2(0.0, 0.0);
                tmp0.xy = tmp0.xy + inp.texcoord.xy;
                tmp1 = tex2D(_DistortionTex, tmp0.xy);
                tmp0.x = _Delay - _AgeSecs;
                tmp0.x = tmp0.x * _ChargeSpeed;
                tmp0.x = frac(tmp0.x);
                tmp0.x = tmp1.x * _DistortionIntensity + tmp0.x;
                tmp0.w = tmp0.x - 0.5;
                tmp0.xy = tmp0.zw + inp.texcoord.xy;
                tmp0 = tex2D(_MainTex, tmp0.xy);
                o.sv_target = saturate(tmp0 * _Color);
                return o;
            }
            ENDCG
            
        }
    }
}
