using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using Verse;
using RimWorld;
using System.Reflection;

namespace OOPhoenixLords
{
    public class AddHediffPatch
    {
        [HarmonyPatch(typeof(Pawn_HealthTracker))]
        [HarmonyPatch(nameof(Pawn_HealthTracker.AddHediff), new Type[] { typeof(Hediff), typeof(BodyPartRecord), typeof(DamageInfo?), typeof(DamageWorker.DamageResult) })]
        static class Pawn_HealthTracker_AddHediff_Patch
        {
            static bool Prefix(Hediff hediff, Pawn ___pawn)
            {
                Pawn pawn = ___pawn;
                Pawn_GeneTracker genes = pawn.genes;
                if (genes != null)
                {
                    foreach (Gene gene in genes.GenesListForReading)
                    {
                        if (gene is GeneWithHediffAndDamageTracking geneWithHediffTracking)
                        {
                            geneWithHediffTracking.PreApplyHediff(hediff, out bool absorbed);
                            if (absorbed)
                            {
                                return false;
                            }
                        }
                    }
                }
                return true;
            }
        }
        [HarmonyPatch(typeof(Pawn_HealthTracker))]
        [HarmonyPatch(nameof(Pawn_HealthTracker.PreApplyDamage))]
        static class Pawn_HealthTracker_PreApplyDamage_Patch
        {
            static void Postfix(DamageInfo dinfo, Pawn ___pawn, ref bool absorbed)
            {
                if (absorbed)
                {
                    return;
                }
                Pawn pawn = ___pawn;
                Pawn_GeneTracker genes = pawn.genes;
                if (genes != null)
                {
                    foreach (Gene gene in genes.GenesListForReading)
                    {
                        if (gene is GeneWithHediffAndDamageTracking geneWithHediffTracking)
                        {
                            geneWithHediffTracking.PreApplyDamage(dinfo, out bool isAbsorbed);
                            if (isAbsorbed)
                            {
                                absorbed = true;
                                return;
                            }
                        }
                    }
                }
            }
        }
    }
}