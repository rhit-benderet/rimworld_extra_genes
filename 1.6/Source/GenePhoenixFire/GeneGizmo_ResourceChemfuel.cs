using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.Sound;
using RimWorld;

namespace OOPhoenixLords
{
	[StaticConstructorOnStartup]
	public class GeneGizmo_ResourceChemfuel : GeneGizmo_Resource
	{
		protected override bool DraggingBar
		{
			get
			{
				return GeneGizmo_ResourceChemfuel.draggingBar;
			}
			set
			{
				GeneGizmo_ResourceChemfuel.draggingBar = value;
			}
		}

		public GeneGizmo_ResourceChemfuel(Gene_Resource gene, List<IGeneResourceDrain> drainGenes, Color barColor, Color barHighlightColor) : base(gene, drainGenes, barColor, barHighlightColor)
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
					// 	GenUI.DrawTextureWithMaterial(rect, GeneGizmo_ResourceChemfuel.ChemfuelCostTex, null, default(Rect));
					// 	GUI.color = Color.white;
					// 	break;
					// }
				}
			}
			return result;
		}

		protected override void DrawHeader(Rect headerRect, ref bool mouseOverElement)
		{
			if (this.IsDraggable)
			{
				Gene_Resource gene = this.gene;
				Gene_PhoenixFire chemfuelGene = gene as Gene_PhoenixFire;
				if (chemfuelGene != null)
				{
					headerRect.xMax -= 24f;
					Rect rect = new Rect(headerRect.xMax, headerRect.y, 24f, 24f);
					Widgets.DefIcon(rect, ThingDefOf.Chemfuel, null, 1f, null, false, null, null, null, 1f);
					GUI.DrawTexture(new Rect(rect.center.x, rect.y, rect.width / 2f, rect.height / 2f), chemfuelGene.chemfuelAllowed ? Widgets.CheckboxOnTex : Widgets.CheckboxOffTex);
					if (Widgets.ButtonInvisible(rect, true))
					{
						chemfuelGene.chemfuelAllowed = !chemfuelGene.chemfuelAllowed;
						if (chemfuelGene.chemfuelAllowed)
						{
							SoundDefOf.Tick_High.PlayOneShotOnCamera(null);
						}
						else
						{
							SoundDefOf.Tick_Low.PlayOneShotOnCamera(null);
						}
					}
					if (Mouse.IsOver(rect))
					{
						Widgets.DrawHighlight(rect);
						string onOff = (chemfuelGene.chemfuelAllowed ? "On" : "Off").Translate().ToString().UncapitalizeFirst();
						TooltipHandler.TipRegion(rect, () => "AutoTakeChemfuelDesc".Translate(this.gene.pawn.Named("PAWN"), chemfuelGene.PostProcessValue(chemfuelGene.targetValue).Named("MIN"), onOff.Named("ONOFF")).Resolve(), 828267373);
						mouseOverElement = true;
					}
				}
			}
			base.DrawHeader(headerRect, ref mouseOverElement);
		}

		protected override string GetTooltip()
		{
			this.tmpDrainGenes.Clear();
			string text = string.Format("{0}: {1} / {2}\n", this.gene.ResourceLabel.CapitalizeFirst().Colorize(ColoredText.TipSectionTitleColor), this.gene.ValueForDisplay, this.gene.MaxForDisplay);
			if (!this.drainGenes.NullOrEmpty<IGeneResourceDrain>())
			{
				float num = 0f;
				foreach (IGeneResourceDrain geneResourceDrain in this.drainGenes)
				{
					if (geneResourceDrain.CanOffset)
					{
						this.tmpDrainGenes.Add(new Pair<IGeneResourceDrain, float>(geneResourceDrain, geneResourceDrain.ResourceLossPerDay));
						num += geneResourceDrain.ResourceLossPerDay;
					}
				}
				if (num != 0f)
				{
					string text2 = (num < 0f) ? "RegenerationRate".Translate() : "DrainRate".Translate();
					text = string.Concat(new string[]
					{
						text,
						"\n\n",
						text2,
						": ",
						"PerDay".Translate(Mathf.Abs(this.gene.PostProcessValue(num))).Resolve()
					});
					foreach (Pair<IGeneResourceDrain, float> pair in this.tmpDrainGenes)
					{
						text = string.Concat(new string[]
						{
							text,
							"\n  - ",
							pair.First.DisplayLabel.CapitalizeFirst(),
							": ",
							"PerDay".Translate(this.gene.PostProcessValue(-pair.Second).ToStringWithSign()).Resolve()
						});
					}
				}
			}
			if (!this.gene.def.resourceDescription.NullOrEmpty())
			{
				text = text + "\n\n" + this.gene.def.resourceDescription.Formatted(this.gene.pawn.Named("PAWN")).Resolve();
			}
			return text;
		}

		private static readonly Texture2D ChemfuelCostTex = SolidColorMaterials.NewSolidColorTexture(new Color(0.78f, 0.72f, 0.66f));

		private const float TotalPulsateTime = 0.85f;

		private static bool draggingBar;

		private List<Pair<IGeneResourceDrain, float>> tmpDrainGenes = new List<Pair<IGeneResourceDrain, float>>();
	}
}
