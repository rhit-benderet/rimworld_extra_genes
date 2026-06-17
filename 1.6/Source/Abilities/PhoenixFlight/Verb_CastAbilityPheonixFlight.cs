using System;
using RimWorld.Utility;
using UnityEngine;
using Verse;
using RimWorld;
using System.Collections.Generic;

namespace OOPhoenixLords
{
	public class Verb_CastAbilityPheonixFlight : Verb_CastAbility
	{
		public override bool MultiSelect
		{
			get
			{
				return true;
			}
		}
		private CompAbilityEffect_AbilityLeaveFlamePath compFlamePath;
		public CompAbilityEffect_AbilityLeaveFlamePath CompFlamePath
		{
			get
			{
				if (this.compFlamePath == null)
				{
					this.compFlamePath = this.Ability.CompOfType<CompAbilityEffect_AbilityLeaveFlamePath>();
				}
				return this.compFlamePath;
			}
		}
		public virtual ThingDef JumpFlyerDef
		{
			get
			{
				return PhoenixLordsThingDefs.OOPhoenixLords_PhoenixFlyer;
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
			return base.TryCastShot() && JumpUtility.DoJump(this.CasterPawn, this.currentTarget, base.ReloadableCompSource, this.verbProps, this.ability, base.CurrentTarget, this.JumpFlyerDef);
		}

		public override void OnGUI(LocalTargetInfo target)
		{
			if (this.CanHitTarget(target) && JumpUtility.ValidJumpTarget(this.CasterPawn, this.caster.Map, target.Cell))
			{
				base.OnGUI(target);
				return;
			}
			GenUI.DrawMouseAttachment(TexCommand.CannotShoot);
		}

		public override void OrderForceTarget(LocalTargetInfo target)
		{
			JumpUtility.OrderJump(this.CasterPawn, target, this, this.EffectiveRange);
		}

		public override bool ValidateTarget(LocalTargetInfo target, bool showMessages = true)
		{
			return this.caster != null && this.CanHitTarget(target) && JumpUtility.ValidJumpTarget(this.CasterPawn, this.caster.Map, target.Cell) && ReloadableUtility.CanUseConsideringQueuedJobs(this.CasterPawn, base.EquipmentSource, true);
		}

		public override bool CanHitTargetFrom(IntVec3 root, LocalTargetInfo targ)
		{
			return JumpUtility.CanHitTargetFrom(this.CasterPawn, root, targ, this.EffectiveRange);
		}

		public override void DrawHighlight(LocalTargetInfo target)
		{
			if (target.IsValid && JumpUtility.ValidJumpTarget(this.CasterPawn, this.caster.Map, target.Cell))
			{
				GenDraw.DrawTargetHighlightWithLayer(target.CenterVector3, AltitudeLayer.MetaOverlays);
				this.ability.DrawEffectPreviews(target);
			}
			GenDraw.DrawRadiusRing(this.caster.Position, this.EffectiveRange, Color.white, (IntVec3 c) => GenSight.LineOfSight(this.caster.Position, c, this.caster.Map) && JumpUtility.ValidJumpTarget(this.caster, this.caster.Map, c));
		}

		private float cachedEffectiveRange = -1f;
    }
}
