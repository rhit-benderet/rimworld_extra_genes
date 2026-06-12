using System;
using System.Collections.Generic;
using OOPhoenixLords;
using Verse;
using RimWorld;
using Verse.Sound;
using UnityEngine;

namespace OOPhoenixLords
{
    public class GeneGizmo_Superheating : Gizmo_Slider
    {
        protected override float Target
        {
            get
            {
                return gene.targetValue / gene.Max;
            }
            set
            {
                gene.SetTargetValuePct(value);
            }
        }
        protected override string BarLabel => $"Heat to {gene.TemperatureSetting}°C";
        protected override Color BarColor { get
            {
                return new ColorInt(201, 0, 0).ToColor;
            }
        }

        protected override Color BarHighlightColor { get
            {
                return new ColorInt(230, 12, 12).ToColor;
            }
        }


        protected override float ValuePercent => Target;
        protected override int Increments => (int)gene.Max;
        protected Gene_Superheating gene;



        protected override string Title
        {
            get
            {
                string text = "Superheating".CapitalizeFirst();
                if (Find.Selector.SelectedPawns.Count != 1)
                {
                    text = text + " (" + gene.pawn.LabelShort + ")";
                }

                return text;
            }
        }

        protected override bool DraggingBar
		{
			get
			{
				return GeneGizmo_Superheating.draggingBar;
			}
			set
			{
				GeneGizmo_Superheating.draggingBar = value;
			}
		}
		private static bool draggingBar;
        public GeneGizmo_Superheating(Gene_Superheating gene)
        {
            this.gene = gene;
        }
        protected override bool IsDraggable
        {
            get
            {
                if (!gene.pawn.IsColonist)
                {
                    return gene.pawn.IsPrisonerOfColony;
                }

                return true;
            }
        }
        protected override void DrawHeader(Rect headerRect, ref bool mouseOverElement)
		{
			if (this.IsDraggable)
			{
				Gene_Superheating gene = this.gene;
				if (gene != null)
				{
					headerRect.xMax -= 24f;
					Rect rect = new Rect(headerRect.xMax, headerRect.y, 24f, 24f);
					Widgets.DefIcon(rect, ThingDefOf.Fire, null, 1f, null, false, null, null, null, 1f);
					GUI.DrawTexture(new Rect(rect.center.x, rect.y, rect.width / 2f, rect.height / 2f), gene.superheatingActive ? Widgets.CheckboxOnTex : Widgets.CheckboxOffTex);
					if (Widgets.ButtonInvisible(rect, true))
					{
						gene.superheatingActive = !gene.superheatingActive;
						if (gene.superheatingActive)
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
						string onOff = (gene.superheatingActive ? "On" : "Off").Translate().ToString().UncapitalizeFirst();
						TooltipHandler.TipRegion(rect, () => "OOPL.SuperheatingActive".Translate(this.gene.pawn.Named("PAWN"), gene.TemperatureSetting.Named("TEMP"), gene.FirePerSecondIfOn.Named("FIRE"), onOff.Named("ONOFF")).Resolve(), 828267373);
						mouseOverElement = true;
					}
				}
			}
			base.DrawHeader(headerRect, ref mouseOverElement);
		}
        protected override string GetTooltip()
        {
            if (!this.gene.ShouldSuperHeat)
            {
                return "Superheating deactivated";
            }
            if (this.gene.PhoenixFireGene.ValueSecondary <= 0)
            {
                return "Not enough phoenix flames to superheat";
            }
            string text = string.Format("Heating to {0}°C\n\nUsing {1} phoenix flames per second", this.gene.TemperatureSetting, this.gene.FirePerSecond);
            return text;
        }
    }
}