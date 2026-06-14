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
            static bool Prefix(Pawn_HealthTracker __instance, Hediff hediff)
            {
                FieldInfo pawnFieldInfo = typeof(Pawn_HealthTracker).GetField("pawn", BindingFlags.NonPublic | BindingFlags.Instance);
                Pawn pawn = (Pawn)pawnFieldInfo.GetValue(__instance);
                Pawn_GeneTracker genes = pawn.genes;
                if (genes != null)
                {
                    foreach (Gene gene in genes.GenesListForReading)
                    {
                        if (gene is GeneWithHediffTracking geneWithHediffTracking)
                        {
                            geneWithHediffTracking.PreApplyHediff(ref hediff, out bool absorbed);
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
    }
}