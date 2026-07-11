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
		private Mote heatWarpMote;
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


        public string Name => "OOPL.Superheating".Translate().CapitalizeFirst();
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
				return this.pawn.Spawned && this.superheatingActive && this.PhoenixFireGene.CanOffset;
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
		public static void HeatWarp(Vector3 c, Map map, float size)
          {
            Vector3 loc = c;
            if (!loc.ShouldSpawnMotesAt(map))
              return;
            Vector3 vector3 = loc + size * new Vector3(Rand.Value - 0.5f, 0.0f, Rand.Value - 0.5f);
            if (!vector3.InBounds(map))
              return;
            FleckCreationData dataStatic = FleckMaker.GetDataStatic(vector3, map, FleckDefOf.FireGlow, Rand.Range(4f, 6f) * size) with
            {
              rotationRate = Rand.Range(-3f, 3f),
              velocityAngle = (float) Rand.Range(0, 360),
              velocitySpeed = 0.12f
            };
            map.flecks.CreateFleck(dataStatic);
          }
			
          public override void Tick()
          {
	          IntVec3 intVec = this.pawn.PositionHeld;
	          if (ShouldSuperHeat && this.PhoenixFireGene.ValueSecondary > 0)
	          {
		          if (heatWarpMote == null || heatWarpMote.Destroyed)
		          {
			          heatWarpMote = MoteMaker.MakeAttachedOverlay(this.pawn,PhoenixLordsMoteDefs.OOPhoenixLords_Mote_SuperHeatingHeatWarp, Vector3.zero);
		          }
		          heatWarpMote?.Maintain();
	          }

          }
        public override void TickInterval(int delta)
		{
			base.TickInterval(delta);
			float heatPerSecond = this.HeatPerSecond;
			bool canHeat = this.CanActuallyHeat;
			if (canHeat)
			{
				GenTemperature.PushHeat(this.pawn.PositionHeld, this.pawn.MapHeld, heatPerSecond * delta / 60f);
			}
		}
    }
}