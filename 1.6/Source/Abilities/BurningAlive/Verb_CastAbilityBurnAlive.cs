using System;
using RimWorld.Utility;
using UnityEngine;
using Verse;
using RimWorld;
using System.Collections.Generic;
using Verse.AI;

namespace OOPhoenixLords
{
	public class Verb_CastAbilityBurnAlive : Verb_CastAbility
	{
		public override bool MultiSelect
		{
			get
			{
				return true;
			}
		}
		public override float EffectiveRange
		{
			get
			{
				if (this.cachedEffectiveRange < 0f)
				{
					this.cachedEffectiveRange = base.EffectiveRange;
				}
				return this.cachedEffectiveRange;
			}
		}

		protected override bool TryCastShot()
		{
			return base.TryCastShot() && BurnInsides(this.CurrentTarget.Pawn);
		}
		private bool BurnInsides(Pawn pawn)
		{
			if (pawn != null)
			{
				if (!pawn.health.hediffSet.HasHediff(PhoenixLordsHediffDefs.OOPhoenixLords_BurningAlive))
				{
					pawn.health.AddHediff(PhoenixLordsHediffDefs.OOPhoenixLords_BurningAlive);
					return true;
				}
			}
			return false;
		}

		public override void OnGUI(LocalTargetInfo target)
		{
			if (this.CanHitTarget(target))
			{
				base.OnGUI(target);
				return;
			}
			GenUI.DrawMouseAttachment(TexCommand.CannotShoot);
		}

		public override void OrderForceTarget(LocalTargetInfo target)
		{
			OrderBurnInsides(this.CasterPawn, target, this);
		}
		public static void OrderBurnInsides(Pawn pawn, LocalTargetInfo target, Verb verb)
		{
			Map map = pawn.Map;
			Job job = JobMaker.MakeJob(PhoenixLordsJobDefs.OOPhoenixLords_BurnInsides, target.Pawn);
			job.verbToUse = verb;
			if (pawn.jobs.TryTakeOrderedJob(job, JobTag.Misc))
			{
				FleckMaker.Static(target.Cell, map, FleckDefOf.FireGlow);
			}
		}

		public override bool ValidateTarget(LocalTargetInfo target, bool showMessages = true)
		{
			return this.caster != null && this.CanHitTarget(target) && IsValidTarget(target.Pawn) && ReloadableUtility.CanUseConsideringQueuedJobs(this.CasterPawn, base.EquipmentSource, true);
		}

		public override bool CanHitTargetFrom(IntVec3 root, LocalTargetInfo targ)
		{
			return GenSight.LineOfSight(root, targ.CenterVector3.ToIntVec3(), this.caster.Map);
		}

		public override bool CanHitTarget(LocalTargetInfo targ)
		{
			return GenSight.LineOfSight(this.caster.Position, targ.CenterVector3.ToIntVec3(), this.caster.Map);
		}
		private bool IsValidTarget(Pawn pawn)
		{
			if (pawn == null)
			{
				return false;
			}
			if (pawn.health.hediffSet.HasHediff(PhoenixLordsHediffDefs.OOPhoenixLords_BurningAlive))
			{
				return false;
			}
			return true;
		}
		public override void DrawHighlight(LocalTargetInfo target)
		{
			if (target.IsValid && target.Pawn != null && GenSight.LineOfSight(this.caster.Position, target.CenterVector3.ToIntVec3(), this.caster.Map))
			{
				GenDraw.DrawTargetHighlightWithLayer(target.CenterVector3, AltitudeLayer.MetaOverlays);
			}
			
			DrawUtils.DrawRingsWithPredicate(this.caster.MapHeld, (Pawn pawn) =>
			{
				if (pawn.Equals(this.Caster)) return false;
				Vector3 c = pawn.DrawPos;
				if (target.IsValid && target.Pawn != null)
				{
					if (target.Pawn.Equals(pawn))
					{
						return false;
					}
				}
				return GenSight.LineOfSight(this.caster.Position, c.ToIntVec3(), this.caster.Map) && IsValidTarget(pawn);
			});
		}
	
		private float cachedEffectiveRange = -1f;
    }
}
