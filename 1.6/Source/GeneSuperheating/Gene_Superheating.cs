using System;
using System.Collections.Generic;
using OOPhoenixLords;
using Verse;
using RimWorld;
using UnityEngine;

namespace OOPhoenixLords
{
	public class Gene_Superheating : Gene, IPhoenixFireSink
    {
        private Gene_PhoenixFire cachedPhoenixFireGene;
		public float targetValue = 0.0f;
		public float Max = 42.0f;
		public bool superheatingActive = false;
		public float Value => targetValue / Max;
		private float? cachedTargetValue;
		private float cachedTemperatureSetting;
		private float cachedFlameSinkAmount;
		public void UpdateCachedValues()
		{
			this.cachedTargetValue = this.targetValue;
			this.cachedTemperatureSetting = getTemperatureSettingFromValue(this.targetValue);
			this.cachedFlameSinkAmount = this.cachedTemperatureSetting / 10f;
		}
		public float getTemperatureSettingFromValue(float value)
		{
			if (value <= 15)
			{
				return value + 15;
			} else if (value <= 29)
			{
				return value * 5 - 45;
			} else if (value <= 37)
			{
				return (value - 29) * 50 + 100;
			} else
			{
				return (value - 37) * 100 + 500;
			}
		}
		public int TemperatureSetting {
			get
			{
				if (this.cachedTargetValue != this.targetValue)
				{
					this.UpdateCachedValues();
				}
				return Mathf.RoundToInt(this.cachedTemperatureSetting);
			}
		}
		public void SetTargetValuePct(float val)
		{
			targetValue = Mathf.Clamp(val * Max, 0f, Max);
		}

        public Gene_PhoenixFire PhoenixFireGene
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

        public float FirePerSecond {
			get
			{
				if (!this.ShouldSuperHeat) return 0f;
				return this.FirePerSecondIfOn;
			}
		}
		public float FirePerSecondIfOn {
			get
			{
				if (this.cachedTargetValue != this.targetValue)
				{
					this.UpdateCachedValues();
				}
				return this.cachedFlameSinkAmount;
			}
		}


        public string Name => "Superheating";
		private GeneGizmo_Superheating gizmo;
        public override IEnumerable<Gizmo> GetGizmos()
		{
			if (!this.Active)
			{
				yield break;
			}
			if (this.gizmo == null)
			{
				this.gizmo = new GeneGizmo_Superheating(this);
			}
			if ((Find.Selector.SelectedPawns.Count == 1 || def.showGizmoOnMultiSelect) && (!pawn.Drafted || def.showGizmoWhenDrafted))
			{
				yield return this.gizmo;
			}
            
			yield break;
		}
		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look<bool>(ref this.superheatingActive, "superheatingActive", true, false);
			Scribe_Values.Look(ref targetValue, "targetValue", 0f);
		}
		private int ticksSinceLastAutomaticShutdown = 1000;
		private bool cachedShouldSuperheat = true;
		public bool CanActuallyHeat
		{
			get
			{
				return this.PhoenixFireGene.ValueSecondary > 0 && ShouldSuperHeat;
			}
		}
		public bool ShouldSuperHeat
		{
			get
			{
				if (!this.superheatingActive) return false;
				bool shouldSuperheat = this.pawn.AmbientTemperature < this.TemperatureSetting;
				if (this.cachedShouldSuperheat)
				{
					if (!shouldSuperheat)
					{
						this.ticksSinceLastAutomaticShutdown = 0;
						this.cachedShouldSuperheat = false;
					}
					return shouldSuperheat;
				} 
				if (this.ticksSinceLastAutomaticShutdown > 180)
				{
					if (shouldSuperheat)
					{
						this.cachedShouldSuperheat = true;
					}
					return shouldSuperheat;
				}
				return this.cachedShouldSuperheat;
			}
		} 
        public override void TickInterval(int delta)
		{
			base.TickInterval(delta);
			float heatPerSecond = this.TemperatureSetting;
			this.ticksSinceLastAutomaticShutdown += delta;
			if (this.CanActuallyHeat)
			{
				GenTemperature.PushHeat(this.pawn.PositionHeld, this.pawn.MapHeld, heatPerSecond * delta / 60f);
			}
		}
    }
}