using System;
using System.Collections.Generic;
using OOPhoenixLords;
using Verse;
using RimWorld;
using PhoenixRebirth;

namespace OOPhoenixLords
{
	public class Gene_BrighterBurn : Gene, IGeneResourceDrain
	{
		public Gene_Resource Resource
		{
			get
			{
				if (this.cachedPhoenixFireGene == null || !this.cachedPhoenixFireGene.Active)
				{
					this.cachedPhoenixFireGene = this.pawn.genes.GetFirstGeneOfType<Gene_PhoenixFire>();
				}
				return this.cachedPhoenixFireGene;
			}
		}

		public bool CanOffset
		{
			get
			{
				return this.Active && !this.pawn.Deathresting && !(this.pawn.ParentHolder is Building_PhoenixAsh) && this.pawn.ageTracker.AgeBiologicalYears >= 3;
			}
		}


		public float ResourceLossPerDay
		{
			get
			{
				return this.def.resourceLossPerDay;
			}
		}

		public Pawn Pawn
		{
			get
			{
				return this.pawn;
			}
		}

		public string DisplayLabel
		{
			get
			{
				return this.Label + " (" + "Gene".Translate() + ")";
			}
		}

		public override void TickInterval(int delta)
		{
			base.TickInterval(delta);
			GeneResourceDrainUtility.TickResourceDrainInterval(this, delta);
		}

		// Token: 0x0400DCFD RID: 56573
		[Unsaved(false)]
		private Gene_PhoenixFire cachedPhoenixFireGene;
	}
}
