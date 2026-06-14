using System;
using System.Collections.Generic;
using Verse;
using RimWorld;

namespace OOPhoenixLords
{
	public class CompAbilityEffect_PhoenixFireBurst : CompAbilityEffect
	{
		private new CompProperties_AbilityPhoenixFireBurst Props
		{
			get
			{
				return (CompProperties_AbilityPhoenixFireBurst)this.props;
			}
		}

		private Pawn Pawn
		{
			get
			{
				return this.parent.pawn;
			}
		}

		public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
		{
			GenExplosion.DoExplosion(this.Pawn.Position, this.Pawn.MapHeld, this.Props.radius, DamageDefOf.Flame, this.Pawn, -1, -1f, null, null, null, null, PhoenixLordsThingDefs.OOPhoenixLords_Filth_SuperheatedChemfuel, 1f, 1, null, null, 255, false, null, 0f, 1, 1f, false, null, null, null, false, 0.6f, 0f, true, null, 1f, null, null, null, null);
			base.Apply(target, dest);
		}

		public override IEnumerable<PreCastAction> GetPreCastActions()
		{
			yield return new PreCastAction
			{
				action = delegate(LocalTargetInfo a, LocalTargetInfo b)
				{
					this.parent.AddEffecterToMaintain(EffecterDefOf.Fire_Burst.Spawn(this.parent.pawn.Position, this.parent.pawn.Map, 1f), this.parent.pawn.Position, 17, this.parent.pawn.Map);
				},
				ticksAwayFromCast = 17
			};
			yield break;
		}

		public override bool AICanTargetNow(LocalTargetInfo target)
		{
			if (this.Pawn.Faction == Faction.OfPlayer)
			{
				return false;
			}
			if (target.HasThing)
			{
				Pawn pawn = target.Thing as Pawn;
				if (pawn != null)
				{
					return pawn.TargetCurrentlyAimingAt == this.Pawn;
				}
			}
			return false;
		}

		public override void CompTickInterval(int delta)
		{
			if (this.parent.Casting)
			{
				ThrowPhoenixFuelTick(this.Pawn.Position, this.Props.radius, this.Pawn.Map);
			}
		}
        public static void ThrowPhoenixFuelTick(IntVec3 position, float radius, Map map)
        {
            if (!Rand.Chance(0.15f))
            {
                return;
            }

            foreach (IntVec3 item in GenRadial.RadialCellsAround(position, radius, useCenter: true).InRandomOrder())
            {
                if (GenSight.LineOfSight(position, item, map, skipFirstCell: true) && FilthMaker.TryMakeFilth(item, map, PhoenixLordsThingDefs.OOPhoenixLords_Filth_SuperheatedChemfuel))
                {
                    break;
                }
            }
        }
	}
}
