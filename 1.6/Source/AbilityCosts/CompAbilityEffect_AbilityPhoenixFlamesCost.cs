using System;
using Verse;
using Verse.AI;
using RimWorld;
using System.Collections.Generic;

namespace OOPhoenixLords
{
	public class CompAbilityEffect_AbilityPhoenixFlamesCost : CompAbilityEffect
	{
		public new CompProperties_AbilityPhoenixFlamesCost Props
		{
			get
			{
				return (CompProperties_AbilityPhoenixFlamesCost)this.props;
			}
		}
		private bool HasEnoughPhoenixFlames
		{
			get
			{
				Pawn_GeneTracker genes = this.parent.pawn.genes;
				Gene_PhoenixFire gene_PhoenixFire = (genes != null) ? genes.GetFirstGeneOfType<Gene_PhoenixFire>() : null;
				return gene_PhoenixFire != null && gene_PhoenixFire.ValueSecondary >= this.Props.phoenixFlamesCost;
			}
		}

		public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
		{
			base.Apply(target, dest);
            Pawn_GeneTracker genes = this.parent.pawn.genes;
            Gene_PhoenixFire gene_PhoenixFire = (genes != null) ? genes.GetFirstGeneOfType<Gene_PhoenixFire>() : null;
			PhoenixFireUtil.OffsetSecondaryResource(gene_PhoenixFire, -this.Props.phoenixFlamesCost);
		}

		public override bool GizmoDisabled(out string reason)
		{
			Pawn_GeneTracker genes = this.parent.pawn.genes;
			Gene_PhoenixFire gene_PhoenixFire = (genes != null) ? genes.GetFirstGeneOfType<Gene_PhoenixFire>() : null;
			if (gene_PhoenixFire == null)
			{
				reason = "OOPL.AbilityDisabledNoPhoenixFlamesGene".Translate(this.parent.pawn);
				return true;
			}
			if (gene_PhoenixFire.ValueSecondary < this.Props.phoenixFlamesCost)
			{
				reason = "OOPL.AbilityDisabledNoPhoenixFlames".Translate(this.parent.pawn);
				return true;
			}
			float num = this.TotalPhoenixFlamesCostOfQueuedAbilities();
			float num2 = this.Props.phoenixFlamesCost + num;
			if (this.Props.phoenixFlamesCost > 1E-45f && num2 > gene_PhoenixFire.ValueSecondary)
			{
				reason = "OOPL.AbilityDisabledNoPhoenixFlames".Translate(this.parent.pawn);
				return true;
			}
			reason = null;
			return false;
		}

		public override bool AICanTargetNow(LocalTargetInfo target)
		{
			return this.HasEnoughPhoenixFlames;
		}
        public static float PhoenixFlamesCost(Ability ability)
        {
            List<AbilityComp> comps = ability.comps;
            if (comps != null)
            {
                foreach (AbilityComp comp in comps)
                {
                    if (comp is CompAbilityEffect_AbilityPhoenixFlamesCost compAbilityEffect_PhoenixFlamesCost)
                    {
                        return compAbilityEffect_PhoenixFlamesCost.Props.phoenixFlamesCost;
                    }
                }
            }

            return 0f;
        }

		private float TotalPhoenixFlamesCostOfQueuedAbilities()
		{
			Pawn_JobTracker jobs = this.parent.pawn.jobs;
			object obj;
			if (jobs == null)
			{
				obj = null;
			}
			else
			{
				Job curJob = jobs.curJob;
				obj = ((curJob != null) ? curJob.verbToUse : null);
			}
			Verb_CastAbility verb_CastAbility = obj as Verb_CastAbility;
			float num;
			if (verb_CastAbility == null)
			{
				num = 0f;
			}
			else
			{
				Ability ability = verb_CastAbility.ability;
				num = ((ability != null) ? PhoenixFlamesCost(ability) : 0f);
			}
			float num2 = num;
			if (this.parent.pawn.jobs != null)
			{
				for (int i = 0; i < this.parent.pawn.jobs.jobQueue.Count; i++)
				{
					Verb_CastAbility verb_CastAbility2 = this.parent.pawn.jobs.jobQueue[i].job.verbToUse as Verb_CastAbility;
					if (verb_CastAbility2 != null)
					{
						float num3 = num2;
						Ability ability2 = verb_CastAbility2.ability;
						num2 = num3 + ((ability2 != null) ? PhoenixFlamesCost(ability2) : 0f);
					}
				}
			}
			return num2;
		}
	}
}
