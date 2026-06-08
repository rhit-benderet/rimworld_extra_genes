using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.AI;
using RimWorld;

namespace OOPhoenixLords
{
	public class JobGiver_GetChemfuel : ThinkNode_JobGiver
	{
		public override float GetPriority(Pawn pawn)
		{
			Pawn_GeneTracker genes = pawn.genes;
			if (((genes != null) ? genes.GetFirstGeneOfType<Gene_PhoenixFire>() : null) == null)
			{
				return 0f;
			}
			return 9.1f;
		}
		protected override Job TryGiveJob(Pawn pawn)
		{
			Pawn_GeneTracker genes = pawn.genes;
			Gene_PhoenixFire gene_PhoenixFire = (genes != null) ? genes.GetFirstGeneOfType<Gene_PhoenixFire>() : null;
			if (gene_PhoenixFire == null)
			{
				return null;
			}
			if (!gene_PhoenixFire.ShouldConsumeChemfuelNow())
			{
				return null;
			}
			if (gene_PhoenixFire.chemfuelAllowed)
			{
				float num = gene_PhoenixFire.Max - gene_PhoenixFire.Value;
				if (num > 0)
				{
					Thing chemfuel = this.GetChemfuel(pawn);
					if (chemfuel != null)
					{
						if (chemfuel.HasComp<Comp_PhoenixFireFuel>())
						{
							float chemFuelAmount = chemfuel.TryGetComp<Comp_PhoenixFireFuel>().Props.refillAmount;
							int grabAmount = Mathf.FloorToInt(num / chemFuelAmount);
							if (grabAmount > 0) {
								Job job = JobMaker.MakeJob(PhoenixLordsJobDefs.OOPhoenixLords_RefuelPhoenixFire, chemfuel);
								job.count = Mathf.Min(chemfuel.stackCount, grabAmount);
								job.ingestTotalCount = true;
								return job;
							}
						}
					}
				}
			}
			return null;
		}
        private static bool IsChemfuel(Thing thing)
        {
            return thing.HasComp<Comp_PhoenixFireFuel>();
        }
		private Thing GetChemfuel(Pawn pawn)
		{
			Thing carriedThing = pawn.carryTracker.CarriedThing;
			if (carriedThing != null && IsChemfuel(carriedThing))
			{
				return carriedThing;
			}
			for (int i = 0; i < pawn.inventory.innerContainer.Count; i++)
			{
				if (IsChemfuel(pawn.inventory.innerContainer[i]))
				{
					return pawn.inventory.innerContainer[i];
				}
			}
			return GenClosest.ClosestThing_Global_Reachable(pawn.Position, pawn.Map, pawn.Map.listerThings.GetAllThings(IsChemfuel), PathEndMode.OnCell, TraverseParms.For(pawn, Danger.Deadly, TraverseMode.ByPawn, false, false, false, true), 9999f, (Thing t) => pawn.CanReserve(t, 1, -1, null, false) && !t.IsForbidden(pawn), null, false);
		}
	}
}
