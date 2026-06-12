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
		private float cachedHeatPerSecond;
		public void UpdateCachedValues()
		{
			this.cachedTargetValue = this.targetValue;
			this.cachedTemperatureSetting = getTemperatureSettingFromValue(this.targetValue);
			this.cachedFlameSinkAmount = this.cachedTemperatureSetting / 2f;
			this.cachedHeatPerSecond = Mathf.Sqrt(this.cachedTemperatureSetting - 15) * 1.6f + 20f;
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
		public bool CanActuallyHeat
		{
			get
			{
				return this.PhoenixFireGene.ValueSecondary > 0 && ShouldSuperHeat && this.pawn.AmbientTemperature < this.TemperatureSetting;
			}
		}
		public bool ShouldSuperHeat
		{
			get
			{
				return this.pawn.Spawned && this.superheatingActive;
			}
		}
		public float HeatPerSecond
		{
			get
			{
				if (this.cachedTargetValue != this.targetValue)
				{
					UpdateCachedValues();
				}
				return this.cachedHeatPerSecond + Mathf.Max(0f, this.pawn.AmbientTemperature / 1.5f);
			}
		}
        public override void TickInterval(int delta)
		{
			base.TickInterval(delta);
			float heatPerSecond = this.HeatPerSecond;
			bool canHeat = this.CanActuallyHeat;
			if (this.ShouldSuperHeat && this.PhoenixFireGene.ValueSecondary > 0)
			{
				if (this.pawn.IsHashIntervalTick(500, delta))
				{
					int flameAmount = Rand.Range(15, 30);
					for (int i = 0; i < flameAmount; i++)
					{
						Vector3 c = this.pawn.DrawPos + new Vector3(Mathf.Cos(2*Mathf.PI*i/flameAmount) + Rand.Range(-0.2f, 0.2f), 0f, Mathf.Sin(2*Mathf.PI*i/flameAmount) + Rand.Range(-0.2f, 0.2f));
						FleckMaker.ThrowFireGlow(c, this.pawn.Map, Rand.Range(Mathf.Min(0.1f + this.HeatPerSecond / 400f, 0.3f), Mathf.Min(0.15f + this.HeatPerSecond / 400f, 0.45f)));
					}
					
				}
			}
			if (canHeat)
			{
				GenTemperature.PushHeat(this.pawn.PositionHeld, this.pawn.MapHeld, heatPerSecond * delta / 60f);
			}
		}
    }
}