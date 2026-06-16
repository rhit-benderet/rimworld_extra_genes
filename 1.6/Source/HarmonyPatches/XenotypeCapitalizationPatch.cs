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
    public class XenotypeCapitalizationPatch
    {
        [HarmonyPatch(typeof(Pawn_GeneTracker))]
        [HarmonyPatch(nameof(Pawn_GeneTracker.XenotypeLabelCap), MethodType.Getter)]
        static class Pawn_GeneTracker_XenotypeLabelCap_Patch
        {
            static void Postfix(ref string __result, Pawn_GeneTracker __instance)
            {
                if (__instance.Xenotype is FullyCapitalizedXenotypeDef)
                {
                    __result =  GenText.ToTitleCaseSmart(__result);
                }
            }
        }
    }
}