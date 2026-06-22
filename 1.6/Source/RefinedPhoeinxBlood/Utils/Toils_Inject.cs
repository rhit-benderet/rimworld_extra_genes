using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.AI;
using RimWorld;

namespace OOPhoenixLords
{
    public class Toils_Inject
    {
        public static Toil AddInjectionEffects(Toil toil, Pawn chewer, TargetIndex ingestibleInd, TargetIndex eatSurfaceInd, Comp_InjectableDrugWithConditions comp)
		{
			toil.WithEffect(delegate()
			{
				LocalTargetInfo target = toil.actor.CurJob.GetTarget(ingestibleInd);
				if (!target.HasThing)
				{
					return null;
				}
				EffecterDef result = comp.Props.injectEffect;
				return result;
			}, delegate()
			{
				if (!toil.actor.CurJob.GetTarget(ingestibleInd).HasThing)
				{
					return null;
				}
				Thing thing = toil.actor.CurJob.GetTarget(ingestibleInd).Thing;
				if (chewer != toil.actor)
				{
					return chewer;
				}
				if (eatSurfaceInd != TargetIndex.None && toil.actor.CurJob.GetTarget(eatSurfaceInd).IsValid)
				{
					return toil.actor.CurJob.GetTarget(eatSurfaceInd);
				}
				return thing;
			}, null);
			toil.PlaySustainerOrSound(delegate()
			{
				if (!chewer.RaceProps.Humanlike)
				{
					return chewer.RaceProps.soundEating;
				}
				LocalTargetInfo target = toil.actor.CurJob.GetTarget(ingestibleInd);
				if (!target.HasThing)
				{
					return null;
				}
				return comp.Props.injectSound;
			}, 1f);
			return toil;
		}
        public static Toil FinalizeInject(Pawn pawn, TargetIndex injectableInd, Comp_InjectableDrugWithConditions comp)
        {
            int duration = comp.Props.baseInjectTicks;
            Toil toil = ToilMaker.MakeToil("FinalizeRefuel");
            toil.initAction = delegate { pawn.jobs.curDriver.ticksLeftThisToil = duration; };
            toil.AddFinishAction(delegate
            {
                Pawn actor = toil.actor;
                Job curJob = actor.jobs.curJob;
                Thing thing = curJob.GetTarget(injectableInd).Thing;
                int num = Mathf.Min(thing.stackCount, curJob.count);
                Inject(comp, ref pawn, ref thing, num);
            }
            );
            toil.WithProgressBar(injectableInd, delegate
            {
                Thing thing = toil.actor.jobs.curJob.GetTarget(injectableInd).Thing;
                return 1f - (float)toil.actor.jobs.curDriver.ticksLeftThisToil / (float)duration;
            }, false, -0.5f, false);
            
            toil.defaultCompleteMode = ToilCompleteMode.Delay;
            return toil;
        }
        private static void Inject(Comp_InjectableDrugWithConditions comp, ref Pawn injecter, ref Thing thing, int amount)
        {
            foreach (HediffDef hediffDef in comp.Props.appliedHediffs)
            {
                HediffGiverUtility.TryApply(injecter, hediffDef, null, true);
            }
            if (thing.stackCount == amount)
            {
                injecter.carryTracker.innerContainer.Remove(thing);
                if (!thing.Destroyed)
                {
                    thing.Destroy(DestroyMode.Vanish);
                }
            } else
            {
                thing.SplitOff(amount);
            }
        }
    }
}