using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using PhoenixRebirth;
namespace OOPhoenixLords
{
	public class Gene_PhoenixFire : Gene_Resource, IGeneResourceDrain
	{
		public virtual int ValueSecondaryForDisplay => PostProcessValue(phoenixFlameCur);

    	public virtual int MaxSecondaryForDisplay => PostProcessValue(phoenixFlameMax);
		public virtual float ValueSecondaryPercent
		{
			get
			{
				return phoenixFlameCur / phoenixFlameMax;
			}
			set
			{
				phoenixFlameCur = phoenixFlameMax * value;
			}
		}
		public IEnumerable<IGeneResourceDrain> GetDrainGenes
		{
			get
			{
				List<Gene> genesListForReading = pawn.genes.GenesListForReading;
				for (int i = 0; i < genesListForReading.Count; i++)
				{
					if (genesListForReading[i] is IGeneResourceDrain geneResourceDrain && geneResourceDrain.Resource == this)
					{
						yield return geneResourceDrain;
					}
				}

				yield break;
			}
		}
		public Gene_Resource Resource
		{
			get
			{
				return this;
			}
		}
    	public virtual float MaxSecondary => phoenixFlameMax;

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
				return this.Active && !this.pawn.Deathresting && !(this.pawn.ParentHolder is Building_PhoenixAsh) && this.pawn.ageTracker.AgeBiologicalYears >= 3;
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
				return 5f;
			}
		}

		public override float MinLevelForAlert
		{
			get
			{
				return -1;
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
				return new ColorInt(207, 112, 13).ToColor;
			}
		}

		protected override Color BarHighlightColor
		{
			get
			{
				return new ColorInt(250, 137, 20).ToColor;
			}
		}
		protected Color BarSecondaryColor
		{
			get
			{
				return new ColorInt(190, 16, 19).ToColor;
			}
		}

		protected Color BarSecondaryHighlightColor
		{
			get
			{
				return new ColorInt(230, 32, 36).ToColor;
			}
		}
		public override void TickInterval(int delta)
		{
			base.TickInterval(delta);
			PhoenixFireUtil.TickResourceDrainInterval(this, pawn, delta);
			if (this.CanOffset)
			{
				if (this.Value > 0.0f)
				{
					this.ticksWithFuel += delta;
				} else
				{
					if (this.ticksWithFuel <= 20 * delta)
					{
						this.ticksWithFuel = 0;
					} else
					{
						this.ticksWithFuel -= 20 * delta;
					}
				}
			}
		}
		
		public virtual float ValueSecondary
		{
			get
			{
				return phoenixFlameCur;
			}
			set
			{
				phoenixFlameCur = Mathf.Clamp(value, 0f, phoenixFlameMax);
			}
		}

		public override void SetTargetValuePct(float val)
		{
			this.targetValue = Mathf.Clamp(val * this.Max, 0f, this.Max - this.MaxLevelOffset);
		}
		public bool ShouldConsumeChemfuelNow()
		{
			return this.Value < this.targetValue;
		}
        public override void PostAdd()
        {
            base.PostAdd();
			if (PawnGenerator.IsBeingGenerated(this.pawn) && !this.pawn.IsColonist && !this.pawn.health.hediffSet.HasHediff(HediffDefOf.CryptosleepSickness))
			{
				this.ticksWithFuel = Rand.Range(Mathf.Min(Mathf.FloorToInt(4 * this.pawn.ageTracker.AgeChronologicalTicks / 10), 900000), Mathf.Min(Mathf.FloorToInt(6 * this.pawn.ageTracker.AgeChronologicalTicks / 10), 3600000));
			}
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
			if (gizmo_secondary == null)
			{
				gizmo_secondary = new GeneGizmo_ResourcePhoenixFlames(this, BarSecondaryColor, BarSecondaryHighlightColor);
			}

			if ((Find.Selector.SelectedPawns.Count == 1 || def.showGizmoOnMultiSelect) && (!pawn.Drafted || def.showGizmoWhenDrafted))
			{
				yield return gizmo_secondary;
			}
			foreach (Gizmo gizmo2 in GeneResourceDrainUtility.GetResourceDrainGizmos(this))
			{
				yield return gizmo2;
			}
			foreach (Gizmo gizmo3 in this.GetBurnTimeGizmos())
			{
				yield return gizmo3;
			}

			yield break;
		}
		public IEnumerable<Gizmo> GetBurnTimeGizmos()
		{
			if (DebugSettings.ShowDevGizmos)
			{
				Command_Action command_Action = new Command_Action();
				command_Action.defaultLabel = "DEV: Burn Time -12 hours";
				command_Action.action = delegate
				{
					this.ticksWithFuel -= 2500 * 12;
					if (this.ticksWithFuel < 0)
					{
						this.ticksWithFuel = 0;
					}
				};
				yield return command_Action;
				Command_Action command_Action2 = new Command_Action();
				command_Action2.defaultLabel = "DEV: Burn Time +12 hours";
				command_Action2.action = delegate
				{
					this.ticksWithFuel += 2500 * 12;
				};
				yield return command_Action2;
			}
		}
		[Unsaved(false)]
		protected GeneGizmo_SecondaryResource gizmo_secondary;
		public void SetFireMax(float newMax)
		{
			phoenixFlameMax = newMax;
			phoenixFlameCur = Mathf.Clamp(phoenixFlameCur, 0f, phoenixFlameMax);
		}
		public float InitialFlameMax = 100.0f;
		public void ResetFireMax()
		{
			phoenixFlameMax = InitialFlameMax;
			phoenixFlameCur = Mathf.Clamp(phoenixFlameCur, 0f, phoenixFlameMax);
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
			Scribe_Values.Look(ref phoenixFlameCur, "phoenixFlameCur", 0f);
			Scribe_Values.Look(ref phoenixFlameMax, "phoenixFlameMax", 0f);
			Scribe_Values.Look(ref ticksWithFuel, "ticksWithFuel", 0);
		}
		public override void Reset()
		{
			phoenixFlameCur = 0;
			phoenixFlameMax = InitialFlameMax;
			base.Reset();
		}
		public bool chemfuelAllowed = true;
		public float phoenixFlameMax;
		public int ticksWithFuel;
		public float phoenixFlameCur;

		public virtual float PostProcessValuePrecise(float value)
		{
			return (float)Mathf.RoundToInt(value * 10000f) / 100f;
		}
	}
}
