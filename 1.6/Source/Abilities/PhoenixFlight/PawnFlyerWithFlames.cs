using RimWorld;

using System;
using System.Reflection;
using Verse;
using Verse.AI;

namespace OOPhoenixLords
{
	public class PawnFlyerWithFlames : PawnFlyer
	{
        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            FieldInfo triggeringAbilityField = typeof(PawnFlyer).GetField("triggeringAbility", BindingFlags.Instance | BindingFlags.NonPublic);
            AbilityDef triggeringAbility = (AbilityDef)triggeringAbilityField.GetValue(this);
            Ability ability = this.FlyingPawn.abilities.GetAbility(triggeringAbility, true);
            if (ability?.comps == null)
            {
                return;
            }
            CompAbilityEffect_AbilityLeaveFlamePath comp = ability.CompOfType<CompAbilityEffect_AbilityLeaveFlamePath>();
            if (comp == null)
            {
                return;
            }
            LocalTargetInfo target = new LocalTargetInfo(this.DestinationPos.ToIntVec3());
            comp.DoExplosion(target, this.ticksFlightTime);
        }
	}
}