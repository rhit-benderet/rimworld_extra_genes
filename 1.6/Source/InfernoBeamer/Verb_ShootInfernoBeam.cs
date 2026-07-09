// Assembly-CSharp, Version=1.6.9676.17735, Culture=neutral, PublicKeyToken=null
// Verse.Verb_ShootBeam
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;
namespace OOPhoenixLords;

public class Verb_ShootInfernoBeam : Verb
{



	private MoteDualAttached mote;

	private Effecter endEffecter;

	private Sustainer sustainer;
	
	private int lastBurstTick;

	protected override int ShotsPerBurst => base.BurstShotCount;

	public Vector3 InterpolatedPosition
	{
		get
		{
			return this.CurrentTarget.CenterVector3;
		}
	}

	public override float? AimAngleOverride
	{
		get
		{
			if (state != VerbState.Bursting)
			{
				return null;
			}
			return (InterpolatedPosition - caster.DrawPos).AngleFlat();
		}
	}

	protected override bool TryCastShot()
	{
		if (currentTarget.HasThing && currentTarget.Thing.Map != caster.Map)
		{
			return false;
		}
		ShootLine resultingLine;
		bool flag = TryFindShootLineFromTo(caster.Position, currentTarget, out resultingLine);
		if (verbProps.stopBurstWithoutLos && !flag)
		{
			return false;
		}
		if (base.EquipmentSource != null)
		{
			base.EquipmentSource.GetComp<CompChangeableProjectile>()?.Notify_ProjectileLaunched();
			base.EquipmentSource.GetComp<CompApparelReloadable>()?.UsedOnce();
		}
		lastShotTick = Find.TickManager.TicksGame;
		IntVec3 targetCell = InterpolatedPosition.Yto0().ToIntVec3();
		if (!TryGetHitCell(resultingLine.Source, targetCell, out var hitCell))
		{
			return true;
		}
		HitCell(hitCell, resultingLine.Source);
		return true;
	}

	protected bool TryGetHitCell(IntVec3 source, IntVec3 targetCell, out IntVec3 hitCell)
	{
		IntVec3 intVec = GenSight.LastPointOnLineOfSight(source, targetCell, (IntVec3 c) => c.InBounds(caster.Map) && c.CanBeSeenOverFast(caster.Map), skipFirstCell: true);
		if (verbProps.beamCantHitWithinMinRange && intVec.DistanceTo(source) < verbProps.minRange)
		{
			hitCell = default(IntVec3);
			return false;
		}
		hitCell = (intVec.IsValid ? intVec : targetCell);
		return intVec.IsValid;
	}

	public override bool TryStartCastOn(LocalTargetInfo castTarg, LocalTargetInfo destTarg, bool surpriseAttack = false, bool canHitNonTargetPawns = true, bool preventFriendlyFire = false, bool nonInterruptingSelfCast = false)
	{
		return this.BaseTryCastOn(castTarg, destTarg, surpriseAttack, canHitNonTargetPawns, preventFriendlyFire, nonInterruptingSelfCast);
	}

	private bool BaseTryCastOn(LocalTargetInfo castTarg, LocalTargetInfo destTarg, bool surpriseAttack = false, bool canHitNonTargetPawns = true, bool preventFriendlyFire = false, bool nonInterruptingSelfCast = false)
	{
		if (caster == null)
		{
			Log.Error("Verb " + GetUniqueLoadID() + " needs caster to work (possibly lost during saving/loading).");
			return false;
		}
		if (!caster.Spawned)
		{
			return false;
		}
		if (!CanHitTarget(castTarg))
		{
			return false;
		}
		if (CausesTimeSlowdown(castTarg))
		{
			Find.TickManager.slower.SignalForceNormalSpeed();
		}
		this.surpriseAttack = surpriseAttack;
		canHitNonTargetPawnsNow = canHitNonTargetPawns;
		this.preventFriendlyFire = preventFriendlyFire;
		this.nonInterruptingSelfCast = nonInterruptingSelfCast;
		currentTarget = castTarg;
		currentDestination = destTarg;
		if (state != VerbState.Bursting)
		{
			int currentTick = Find.TickManager.TicksGame;
			if (currentTick <= this.lastBurstTick + 22)
			{
				if (verbTracker.directOwner is Ability ability)
				{
					ability.lastCastTick = Find.TickManager.TicksGame;
				}
				WarmupComplete();
				return true;
			}
			if (CasterIsPawn && WarmupTime > 0f)
			{
				if (!TryFindShootLineFromTo(caster.Position, castTarg, out var resultingLine))
				{
					return false;
				}

				CasterPawn.Drawer.Notify_WarmingCastAlongLine(resultingLine, caster.Position);
				float statValue = CasterPawn.GetStatValue(StatDefOf.AimingDelayFactor);
				int ticks = (WarmupTime * statValue).SecondsToTicks();
				CasterPawn.stances.SetStance(new Stance_Warmup(ticks, castTarg, this));
				if (verbProps.stunTargetOnCastStart && castTarg.Pawn != null)
				{
					castTarg.Pawn.stances.stunner.StunFor(ticks, null, addBattleLog: false);
				}
			}
			else
			{
				if (verbTracker.directOwner is Ability ability)
				{
					ability.lastCastTick = Find.TickManager.TicksGame;
				}

				WarmupComplete();
			}
		}

		return true;
	}
	
	public override void BurstingTick()
	{
		this.lastBurstTick = Find.TickManager.TicksGame;
		if (this.burstShotsLeft == 0)
		{
			this.CasterPawn.stances.SetStance(new Stance_Mobile());
		}
		Vector3 vector = InterpolatedPosition;
		IntVec3 intVec = vector.ToIntVec3();
		Vector3 vector2 = InterpolatedPosition - caster.Position.ToVector3Shifted();
		float num = vector2.MagnitudeHorizontal();
		Vector3 normalized = vector2.Yto0().normalized;
		IntVec3 intVec2 = GenSight.LastPointOnLineOfSight(caster.Position, intVec, (IntVec3 c) => c.CanBeSeenOverFast(caster.Map), skipFirstCell: true);
		if (intVec2.IsValid)
		{
			num -= (intVec - intVec2).LengthHorizontal;
			vector = caster.Position.ToVector3Shifted() + normalized * num;
			intVec = vector.ToIntVec3();
		}
		Vector3 offsetA = normalized * verbProps.beamStartOffset;
		Vector3 vector3 = vector - intVec.ToVector3Shifted();
		if (mote != null)
		{
			mote.Maintain();
		}
		if (endEffecter == null && verbProps.beamEndEffecterDef != null)
		{
			endEffecter = verbProps.beamEndEffecterDef.Spawn(intVec, caster.Map, vector3);
		}
		if (endEffecter != null)
		{
			endEffecter.offset = vector3;
			endEffecter.EffectTick(new TargetInfo(intVec, caster.Map), TargetInfo.Invalid);
			endEffecter.ticksLeft--;
		}
		if (verbProps.beamLineFleckDef != null)
		{
			float num2 = 1f * num;
			for (int num3 = 0; (float)num3 < num2; num3++)
			{
				if (Rand.Chance(verbProps.beamLineFleckChanceCurve.Evaluate((float)num3 / num2)))
				{
					Vector3 vector4 = num3 * normalized - normalized * Rand.Value + normalized / 2f;
					FleckMaker.Static(caster.Position.ToVector3Shifted() + vector4, caster.Map, verbProps.beamLineFleckDef);
				}
			}
		}
		sustainer?.Maintain();
		// if (this.ticksToNextBurstShot == this.TicksBetweenBurstShots)
		// {
		// 	this.CasterPawn.stances.SetStance((Stance) new Stance_Cooldown(this.TicksBetweenBurstShots + 1, this.currentTarget, this));
		// }
		
	}

	public override void ExposeData()
	{
		base.ExposeData();
		Scribe_Values.Look(ref lastBurstTick, "lastBurstTick");
	}

	public override void WarmupComplete()
	{
		burstShotsLeft = ShotsPerBurst;
		state = VerbState.Bursting;
		if (verbProps.beamMoteDef != null)
		{
			mote = MoteMaker.MakeInteractionOverlay(verbProps.beamMoteDef, caster, CurrentTarget.ToTargetInfo(caster.Map));
		}
		TryCastNextBurstShot();
		endEffecter?.Cleanup();
		if (verbProps.soundCastBeam != null)
		{
			sustainer = verbProps.soundCastBeam.TrySpawnSustainer(SoundInfo.InMap(caster, MaintenanceType.PerTick));
		}
	}
	private void HitCell(IntVec3 cell, IntVec3 sourceCell, float damageFactor = 1f)
	{
		if (CurrentTarget.Thing == null) return;
		if (cell.InBounds(caster.Map))
		{
			ApplyDamage(CurrentTarget.Thing, sourceCell, damageFactor);
			if (verbProps.beamSetsGroundOnFire && Rand.Chance(verbProps.beamChanceToStartFire))
			{
				FireUtility.TryStartFireIn(cell, caster.Map, 1f, caster);
			}
		}
	}

	private void ApplyDamage(Thing thing, IntVec3 sourceCell, float damageFactor = 1f)
	{
		IntVec3 intVec = InterpolatedPosition.Yto0().ToIntVec3();
		IntVec3 intVec2 = GenSight.LastPointOnLineOfSight(sourceCell, intVec, (IntVec3 c) => c.InBounds(caster.Map) && c.CanBeSeenOverFast(caster.Map), skipFirstCell: true);
		if (intVec2.IsValid)
		{
			intVec = intVec2;
		}
		Map map = caster.Map;
		if (thing == null || verbProps.beamDamageDef == null)
		{
			return;
		}
		float angleFlat = (currentTarget.Cell - caster.Position).AngleFlat;
		BattleLogEntry_RangedImpact log = new BattleLogEntry_RangedImpact(caster, thing, currentTarget.Thing, base.EquipmentSource.def, null, null);
		DamageInfo dinfo;
		if (verbProps.beamTotalDamage > 0f)
		{
			float num = verbProps.beamTotalDamage / (float)verbProps.burstShotCount;
			num *= damageFactor;
			dinfo = new DamageInfo(verbProps.beamDamageDef, num, verbProps.beamDamageDef.defaultArmorPenetration, angleFlat, caster, null, base.EquipmentSource.def, DamageInfo.SourceCategory.ThingOrUnknown, currentTarget.Thing);
		}
		else
		{
			float amount = (float)verbProps.beamDamageDef.defaultDamage * damageFactor;
			dinfo = new DamageInfo(verbProps.beamDamageDef, amount, verbProps.beamDamageDef.defaultArmorPenetration, angleFlat, caster, null, base.EquipmentSource.def, DamageInfo.SourceCategory.ThingOrUnknown, currentTarget.Thing);
		}
		thing.TakeDamage(dinfo).AssociateWithLog(log);
		if (thing.CanEverAttachFire())
		{
			float chance = ((verbProps.flammabilityAttachFireChanceCurve == null) ? verbProps.beamChanceToAttachFire : verbProps.flammabilityAttachFireChanceCurve.Evaluate(thing.GetStatValue(StatDefOf.Flammability)));
			if (Rand.Chance(chance))
			{
				thing.TryAttachFire(verbProps.beamFireSizeRange.RandomInRange, caster);
			}
		}
		else if (Rand.Chance(verbProps.beamChanceToStartFire))
		{
			FireUtility.TryStartFireIn(intVec, map, verbProps.beamFireSizeRange.RandomInRange, caster, verbProps.flammabilityAttachFireChanceCurve);
		}
	}
}
