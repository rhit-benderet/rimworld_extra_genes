using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.Sound;
using RimWorld;

namespace OOPhoenixLords
{
    public abstract class GeneGizmo_SecondaryResource : Gizmo_Slider
    {
        protected Gene_PhoenixFire gene;


        protected override Color BarColor { get; }

        protected override Color BarHighlightColor { get; }

        protected override bool IsDraggable
        {
            get
            {
                return false;
            }
        }

        protected override string BarLabel => $"{gene.ValueSecondaryForDisplay} / {gene.MaxSecondaryForDisplay}";

        protected override int Increments => gene.MaxSecondaryForDisplay / 10;

        protected override float ValuePercent => gene.ValueSecondaryPercent;

        protected override FloatRange DragRange => new FloatRange(0f, gene.Max);

        protected override float Target
        {
            get
            {
                return 1.0f;
            }
            set
            {
            }
        }

        protected override string Title
        {
            get
            {
                string text = "OOPL.PhoenixFlames".Translate().CapitalizeFirst();
                if (Find.Selector.SelectedPawns.Count != 1)
                {
                    text = text + " (" + gene.pawn.LabelShort + ")";
                }

                return text;
            }
        }

        public GeneGizmo_SecondaryResource(Gene_PhoenixFire gene, Color barColor, Color barHighlightColor)
        {
            this.gene = gene;
            BarColor = barColor;
            BarHighlightColor = barHighlightColor;
        }

        protected override IEnumerable<float> GetBarThresholds()
        {
            for (int i = 0; i < gene.def.resourceGizmoThresholds.Count; i++)
            {
                yield return gene.def.resourceGizmoThresholds[i];
            }
        }
    }
}