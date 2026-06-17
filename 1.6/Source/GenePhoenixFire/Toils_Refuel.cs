using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.AI;
using RimWorld;

namespace OOPhoenixLords
{
    public class Toils_Refuel
    {
        public static Toil FinalizeRefuel(Pawn ingester, TargetIndex ingestibleInd)
        {
            int duration = 300;
            Toil toil = ToilMaker.MakeToil("FinalizeRefuel");
            toil.initAction = delegate { ingester.jobs.curDriver.ticksLeftThisToil = duration; };
            toil.AddFinishAction(delegate
            {
                Pawn actor = toil.actor;
                Job curJob = actor.jobs.curJob;
                Thing thing = curJob.GetTarget(ingestibleInd).Thing;
                Gene_PhoenixFire chemfuelGene = ingester.genes.GetGene(PhoenixLordsGeneDefs.OOPhoenixLords_PhoenixFire) as Gene_PhoenixFire;
                int num = Mathf.Min(thing.stackCount, curJob.count);
                ChemfuelConsumptionUtil.Refuel(ref chemfuelGene, ref thing, num, ref ingester);
            }
            );
            toil.WithProgressBar(ingestibleInd, delegate
            {
                Thing thing = toil.actor.jobs.curJob.GetTarget(ingestibleInd).Thing;
                return 1f - (float)toil.actor.jobs.curDriver.ticksLeftThisToil / (float)duration;
            }, false, -0.5f, false);
            
            toil.defaultCompleteMode = ToilCompleteMode.Delay;
            return toil;
        }
    }
}