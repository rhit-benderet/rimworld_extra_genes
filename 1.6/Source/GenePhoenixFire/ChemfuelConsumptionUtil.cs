using RimWorld;
using UnityEngine;
using Verse;
namespace OOPhoenixLords
{
    public static class ChemfuelConsumptionUtil
    {
        public static bool IsChemfuel(Thing thing)
        {
            return thing.HasComp<Comp_PhoenixFireFuel>();
        }
        public static void Refuel(ref Gene_PhoenixFire chemfuelGene, ref Thing thing, int amount, ref Pawn ingester)
        {
            if (chemfuelGene != null) {
                chemfuelGene.Refuel(thing, amount);
            }
            if (thing.stackCount == amount)
            {
                ingester.carryTracker.innerContainer.Remove(thing);
                if (!thing.Destroyed)
                {
                    thing.Destroy(DestroyMode.Vanish);
                }
            } else
            {
                thing.SplitOff(amount);
            }
        }
        public static int getChemfuelAmount(Gene_PhoenixFire gene_PhoenixFire, Thing thing)
        {
            float num = gene_PhoenixFire.Max - gene_PhoenixFire.Value;
            Comp_PhoenixFireFuel comp_PhoenixFireFuel = thing.TryGetComp<Comp_PhoenixFireFuel>();
            if (comp_PhoenixFireFuel == null)
            {
                return 0;
            }
            int amount = Mathf.FloorToInt(num / comp_PhoenixFireFuel.Props.refillAmount);
            return Mathf.Min(amount, thing.stackCount);
        }
    }
}