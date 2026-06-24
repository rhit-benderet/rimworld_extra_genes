using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace OOPhoenixLords
{
    public static class PhoenixFireUtil
    {
        public class PhoenixFireOffsetsPerSecond
        {
            public string displayName;
            public float offset;
            public PhoenixFireOffsetsPerSecond(string displayName, float offset)
            {
                this.displayName = displayName;
                this.offset = offset;
            }
        }
        public static IEnumerable<PhoenixFireOffsetsPerSecond> CalculateOffsets(Gene_PhoenixFire gene, Pawn pawn)
        {
            if (!gene.CanOffset) yield break;
            if (gene.Value > 0.0f)
            {
                float burningAmount = ComputeBurningAmount(gene, pawn);
                yield return new PhoenixFireOffsetsPerSecond("OOPL.ChemfuelBurning".Translate().CapitalizeFirst(), burningAmount);
                HediffSet hediffSet = pawn.health.hediffSet;
                IEnumerable<HediffComp_PhoenixFlameMultiplier> comps = hediffSet.GetHediffComps<HediffComp_PhoenixFlameMultiplier>();
                foreach (HediffComp_PhoenixFlameMultiplier comp in comps)
                {
                    yield return new PhoenixFireOffsetsPerSecond(comp.HediffLabelCap, comp.Multiplier * burningAmount);
                }
            }
            yield return new PhoenixFireOffsetsPerSecond("OOPL.Entropy".Translate().CapitalizeFirst(), -gene.phoenixFlameCur * 0.1f);
            foreach (IPhoenixFireSink phoenixFireSink in pawn.genes.GenesListForReading.OfType<IPhoenixFireSink>())
            {
                if (phoenixFireSink.FirePerSecond != 0f)
                {
                    yield return new PhoenixFireOffsetsPerSecond(phoenixFireSink.Name, -phoenixFireSink.FirePerSecond / 100f);
                }
            }
            yield break;
        }
        public static float TotalOffset(Gene_PhoenixFire gene, Pawn pawn)
        {
            float num = 0f;
            foreach (PhoenixFireOffsetsPerSecond item in CalculateOffsets(gene, pawn))
            {
                num += item.offset;
            }
            return num;
        }
        private static float GetDrainRate(Gene_PhoenixFire gene, Pawn pawn)
        {
            float num = 0f;
            foreach (IGeneResourceDrain drainGene in gene.GetDrainGenes)
            {
                if (drainGene.CanOffset)
                {
                    num += drainGene.ResourceLossPerDay;
                }
            }
            return num;
        }
        public static float ComputeBurningAmount(Gene_PhoenixFire gene, Pawn pawn)
        {
            float baseAmount = ComputeBurningAmountBase(gene, pawn);
            return baseAmount * GetDrainRate(gene, pawn) / 0.2f;
        }
        public static float ComputeBurningAmountBase(Gene_PhoenixFire gene, Pawn pawn)
        {
            if (gene.ticksWithFuel > 6 * 60000)
            {
                return (gene.ticksWithFuel - 6f * 60000f) / 2000000f + 0.55f;
            } else
            {
                SimpleCurve curve = new SimpleCurve
                {
                    new CurvePoint(0f, 0.0f),
                    new CurvePoint(40000f, 0.02f),
                    new CurvePoint(60000f, 0.04f),
                    new CurvePoint(120000f, 0.1f),
                    new CurvePoint(180000f, 0.2f),
                    new CurvePoint(240000f, 0.35f),
                    new CurvePoint(300000f, 0.475f),
                    new CurvePoint(360000f, 0.55f),
                };
                return curve.Evaluate(gene.ticksWithFuel);
            }
        }
        public static void TickResourceDrainInterval(Gene_PhoenixFire drain, Pawn pawn, int delta)
        {
            if (drain.CanOffset && drain.Resource != null)
            {
                OffsetResource(drain, (0f - drain.ResourceLossPerDay) * (float)delta / 60000f);
            }
             if (drain.CanOffset)
            {
                OffsetSecondaryResource(drain, TotalOffset(drain, pawn) * (float)delta / 60f);
            }
        }

        public static void OffsetResource(Gene_PhoenixFire drain, float amnt)
        {
            if (drain.Resource != null)
            {
                float value = drain.Resource.Value;
                drain.Resource.Value += amnt;
            }
        }
        public static void OffsetSecondaryResource(Gene_PhoenixFire drain, float amnt)
        {
            if (drain != null)
            {
                float value = drain.ValueSecondary;
                drain.ValueSecondary += amnt;
            }
        }
    }
}