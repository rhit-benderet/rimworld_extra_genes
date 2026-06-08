using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.Sound;
using RimWorld;
using static OOPhoenixLords.PhoenixFireUtil;

namespace OOPhoenixLords
{
	[StaticConstructorOnStartup]
	public class GeneGizmo_ResourcePhoenixFlames : GeneGizmo_SecondaryResource
	{
		protected override bool DraggingBar
		{
			get
			{
				return GeneGizmo_ResourcePhoenixFlames.draggingBar;
			}
			set
			{
				GeneGizmo_ResourcePhoenixFlames.draggingBar = value;
			}
		}

		public GeneGizmo_ResourcePhoenixFlames(Gene_PhoenixFire gene, Color barColor, Color barHighlightColor) : base(gene, barColor, barHighlightColor)
		{
		}

		public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth, GizmoRenderParms parms)
		{
			GizmoResult result = base.GizmoOnGUI(topLeft, maxWidth, parms);
			float num = Mathf.Repeat(Time.time, 0.85f);
			float num2 = 1f;
			if (num < 0.1f)
			{
				num2 = num / 0.1f;
			}
			else if (num >= 0.25f)
			{
				num2 = 1f - (num - 0.25f) / 0.6f;
			}
			MainTabWindow_Inspect mainTabWindow_Inspect = (MainTabWindow_Inspect)MainButtonDefOf.Inspect.TabWindow;
			Command_Ability command_Ability = MapGizmoUtility.LastMouseOverGizmo as Command_Ability;
			if (command_Ability != null && this.gene.Max != 0f)
			{
				foreach (CompAbilityEffect compAbilityEffect in command_Ability.Ability.EffectComps)
				{
					// CompAbilityEffect_RequiresChemfuel compAbilityEffect_ChemfuelCost = compAbilityEffect as CompAbilityEffect_RequiresChemfuel;
					// if (compAbilityEffect_ChemfuelCost != null && compAbilityEffect_ChemfuelCost.Props.phoenixChemfuelCost > 1E-45f)
					// {
					// 	Rect rect = this.barRect.ContractedBy(3f);
					// 	float width = rect.width;
					// 	float num3 = this.gene.Value / this.gene.Max;
					// 	rect.xMax = rect.xMin + width * num3;
					// 	float num4 = Mathf.Min(compAbilityEffect_ChemfuelCost.Props.phoenixChemfuelCost / this.gene.Max, 1f);
					// 	rect.xMin = Mathf.Max(rect.xMin, rect.xMax - width * num4);
					// 	GUI.color = new Color(1f, 1f, 1f, num2 * 0.7f);
					// 	GenUI.DrawTextureWithMaterial(rect, GeneGizmo_ResourcePhoenixFlames.ChemfuelCostTex, null, default(Rect));
					// 	GUI.color = Color.white;
					// 	break;
					// }
				}
			}
			return result;
		}

		protected override void DrawHeader(Rect headerRect, ref bool mouseOverElement)
		{
			base.DrawHeader(headerRect, ref mouseOverElement);
		}

		protected override string GetTooltip()
		{
		// 	this.tmpDrainGenes.Clear();
            List<PhoenixFireOffsetsPerSecond> list = new List<PhoenixFireOffsetsPerSecond>();
			string text = string.Format("{0}: {1} / {2}\n", "Phoenix Flames".CapitalizeFirst().Colorize(ColoredText.TipSectionTitleColor), this.gene.ValueSecondaryForDisplay, this.gene.MaxSecondaryForDisplay);
            float num = 0f;
            foreach (PhoenixFireOffsetsPerSecond item in PhoenixFireUtil.CalculateOffsets(this.gene, this.gene.pawn))
            {
                num += item.offset;
                list.Add(item);
            }
            string text2 = (num > 0f) ? "RegenerationRate".Translate() : "DrainRate".Translate();
            text = string.Concat(new string[]
            {
                text,
                "\n\n",
                text2,
                ": ",
                "OOPL.PerSecond".Translate(Mathf.Abs(this.gene.PostProcessValuePrecise(num)).ToString()).Resolve()
            });
            foreach (PhoenixFireOffsetsPerSecond pair in list)
            {
                text = string.Concat(new string[]
                {
                    text,
                    "\n  - ",
                    pair.displayName.CapitalizeFirst(),
                    ": ",
                    "OOPL.PerSecond".Translate(this.gene.PostProcessValuePrecise(pair.offset).ToStringWithSign()).Resolve()
                });
            }
			if (!this.gene.def.resourceDescription.NullOrEmpty())
			{
				text = text + "\n\n" + this.gene.def.resourceDescription.Formatted(this.gene.pawn.Named("PAWN")).Resolve();
			}
			return text;
		}

		private static readonly Texture2D PhoenixFlamesCostTex = SolidColorMaterials.NewSolidColorTexture(new Color(0.78f, 0.72f, 0.00f));

		private const float TotalPulsateTime = 0.85f;

		private static bool draggingBar;

		private List<Pair<IGeneResourceDrain, float>> tmpDrainGenes = new List<Pair<IGeneResourceDrain, float>>();
	}
}
