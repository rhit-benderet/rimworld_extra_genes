using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.AI;
using RimWorld;

namespace OOPhoenixLords
{
	public class JobDriver_RefuelPhoenixFire : JobDriver
	{
		private Thing IngestibleSource
		{
			get
			{
				return this.job.GetTarget(TargetIndex.A).Thing;
			}
		}
		public bool EatingFromInventory
		{
			get
			{
				return this.refuelingFromInventory;
			}
		}

        public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look<bool>(ref this.refuelingFromInventory, "refuelingFromInventory", false, false);
		}

		public override string GetReport()
		{
			return base.GetReport();
		}

		public override void Notify_Starting()
		{
			base.Notify_Starting();
			this.refuelingFromInventory = (this.pawn.inventory != null && this.pawn.inventory.Contains(this.IngestibleSource));
		}

		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			if (this.pawn.Faction != null)
			{
				Thing ingestibleSource = this.IngestibleSource;
				int maxAmountToPickup = Math.Min(ingestibleSource.stackCount, this.job.count);
				if (!this.pawn.Reserve(ingestibleSource, this.job, 10, maxAmountToPickup, null, errorOnFailed, false))
				{
					return false;
				}
			}
			return true;
		}
        private Toil ReserveChemfuel()
		{
			Toil toil = ToilMaker.MakeToil("ReserveChemfuel");
			toil.initAction = delegate()
			{
				if (this.pawn.Faction == null)
				{
					return;
				}
				Thing thing = this.job.GetTarget(TargetIndex.A).Thing;
				if (this.pawn.carryTracker.CarriedThing == thing)
				{
					return;
				}
				int maxAmountToPickup = Math.Min(thing.stackCount, this.job.count);
				if (maxAmountToPickup == 0)
				{
					return;
				}
				if (!this.pawn.Reserve(thing, this.job, 10, maxAmountToPickup, null, true, false))
				{
					string[] array = new string[8];
					array[0] = "Pawn chemfuel reservation for ";
					int num = 1;
					Pawn pawn = this.pawn;
					array[num] = ((pawn != null) ? pawn.ToString() : null);
					array[2] = " on job ";
					array[3] = ((this != null) ? this.ToString() : null);
					array[4] = " failed, because it could not register chemfuel from ";
					int num2 = 5;
					Thing thing2 = thing;
					array[num2] = ((thing2 != null) ? thing2.ToString() : null);
					array[6] = " - amount: ";
					array[7] = maxAmountToPickup.ToString();
					Log.Error(string.Concat(array));
					this.pawn.jobs.EndCurrentJob(JobCondition.Errored, true, true);
				}
				this.job.count = maxAmountToPickup;
			};
			toil.defaultCompleteMode = ToilCompleteMode.Instant;
			toil.atomicWithPrevious = true;
			return toil;
		}
        private IEnumerable<Toil> PrepareToIngestToils()
		{
			yield return this.ReserveChemfuel();
			yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch, false);
			yield break;
		}

		protected override IEnumerable<Toil> MakeNewToils()
		{
            foreach (Toil toil in this.PrepareToIngestToils())
			{
				yield return toil;
			}
			yield return Toils_Refuel.FinalizeRefuel(this.pawn, TargetIndex.A);
			yield break;
		}
		public override bool ModifyCarriedThingDrawPos(ref Vector3 drawPos, ref bool flip)
		{
			IntVec3 cell = this.job.GetTarget(TargetIndex.B).Cell;
			return JobDriver_Ingest.ModifyCarriedThingDrawPosWorker(ref drawPos, ref flip, cell, this.pawn);
		}


		private bool refuelingFromInventory;

		public const TargetIndex IngestibleSourceInd = TargetIndex.A;
	}
}
