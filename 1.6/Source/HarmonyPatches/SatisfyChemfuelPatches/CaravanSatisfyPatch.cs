using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using Verse;
using RimWorld;
using System.Reflection;
using RimWorld.Planet;

namespace OOPhoenixLords
{
    public class CaravanSatisfyPatch
    {
        [HarmonyPatch(typeof(Caravan_NeedsTracker))]
        [HarmonyPatch("TrySatisfyPawnNeeds", new Type[] {typeof(Pawn), typeof(int)})]
        static class Caravan_NeedsTracker_TrySatisfyPawnNeeds_Patch
        {
            static void Postfix(ref Pawn pawn, ref int delta, ref Caravan_NeedsTracker __instance)
            {
                if (ModsConfig.BiotechActive && pawn.genes != null)
                {
                    Gene_PhoenixFire firstGeneOfType = pawn.genes.GetFirstGeneOfType<Gene_PhoenixFire>();
                    if (firstGeneOfType != null)
                    {
                        TrySatisfyChemfuelNeed(__instance, pawn, firstGeneOfType, delta);
                    }
                }
            }
        }
        static void TrySatisfyChemfuelNeed(Caravan_NeedsTracker instance, Pawn pawn, Gene_PhoenixFire phoenixFireGene, int delta)
        {
			if (phoenixFireGene.ShouldConsumeChemfuelNow())
			{
				Thing thing = CaravanInventoryUtility.AllInventoryItems(instance.caravan).FirstOrFallback(ChemfuelConsumptionUtil.IsChemfuel);
				if (thing != null)
				{
					Pawn ownerOf = CaravanInventoryUtility.GetOwnerOf(instance.caravan, thing);
                    int amount = ChemfuelConsumptionUtil.getChemfuelAmount(phoenixFireGene, thing);
                    ChemfuelConsumptionUtil.Refuel(ref phoenixFireGene, ref thing, amount, ref pawn);
                    if (thing.Destroyed && ownerOf != null)
					{
						ownerOf.inventory.innerContainer.Remove(thing);
						instance.caravan.RecacheInventory();
					}
				}
			}
        }
    }
}