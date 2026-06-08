using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
namespace OOPhoenixLords
{
	public class Gene_PhoenixFire : Gene_Resource, IGeneResourceDrain
	{
		public Gene_Resource Resource
		{
			get
			{
				return this;
			}
		}

		public Pawn Pawn
		{
			get
			{
				return this.pawn;
			}
		}

		public bool CanOffset
		{
			get
			{
				return this.Active && !this.pawn.Deathresting;
			}
		}

		public string DisplayLabel
		{
			get
			{
				return this.Label + " (" + "Gene".Translate() + ")";
			}
		}

		public float ResourceLossPerDay
		{
			get
			{
				return this.def.resourceLossPerDay;
			}
		}

		public override float InitialResourceMax
		{
			get
			{
				return 1f;
			}
		}

		public override float MinLevelForAlert
		{
			get
			{
				return 0.15f;
			}
		}
		public override float MaxLevelOffset
		{
			get
			{
				return 0.1f;
			}
		}

		protected override Color BarColor
		{
			get
			{
				return new ColorInt(138, 138, 3).ToColor;
			}
		}

		protected override Color BarHighlightColor
		{
			get
			{
				return new ColorInt(145, 145, 42).ToColor;
			}
		}

		public override void TickInterval(int delta)
		{
			base.TickInterval(delta);
			GeneResourceDrainUtility.TickResourceDrainInterval(this, delta);
		}

		public override void SetTargetValuePct(float val)
		{
			this.targetValue = Mathf.Clamp(val * this.Max, 0f, this.Max - this.MaxLevelOffset);
		}
		public bool ShouldConsumeChemfuelNow()
		{
			return this.Value < this.targetValue;
		}
		public override IEnumerable<Gizmo> GetGizmos()
		{
			if (!this.Active)
			{
				yield break;
			}
			foreach (Gizmo gizmo in base.GetGizmos())
			{
				yield return gizmo;
			}
			foreach (Gizmo gizmo2 in GeneResourceDrainUtility.GetResourceDrainGizmos(this))
			{
				yield return gizmo2;
			}
			yield break;
		}
		public void Refuel(Thing thing, int numTaken)
		{
			if (thing.def.HasComp(typeof(Comp_PhoenixFireFuel)))
			{
				CompProperties_PhoenixFireFuel compProperties_PhoenixFireFuel = thing.def.GetCompProperties<CompProperties_PhoenixFireFuel>();
				this.Value = Mathf.Clamp(this.Value + numTaken * compProperties_PhoenixFireFuel.refillAmount, 0f, this.Max);
			}
		}
		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look<bool>(ref this.chemfuelAllowed, "chemfuelAllowed", true, false);
		}
		public bool chemfuelAllowed = true;

	}
}
