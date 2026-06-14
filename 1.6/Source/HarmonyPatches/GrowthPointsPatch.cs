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
    public class GrowthPointsPatch
    {
        [HarmonyPatch(typeof(Pawn_AgeTracker))]
        [HarmonyPatch(nameof(Pawn_AgeTracker.GrowthPointsPerDay), MethodType.Getter)]
        static class Pawn_AgeTracker_GrowthPointsPerDay_Patch
        {
            static void Postfix(ref float __result, Pawn_AgeTracker __instance)
            {
                FieldInfo pawnFieldInfo = typeof(Pawn_AgeTracker).GetField("pawn", BindingFlags.NonPublic | BindingFlags.Instance);
                Pawn pawn = (Pawn)pawnFieldInfo.GetValue(__instance);
                float factor = pawn.GetStatValue(PhoenixLordsStatDefs.OOPhoenixLords_GrowthPointsFactor, true);
                __result *= factor;
            }
        }
    }
}