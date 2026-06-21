using System;
using System.Collections.Generic;
using Verse;
using RimWorld;
using Verse.AI;

namespace OOPhoenixLords
{
    public class Comp_InjectableDrugWithConditions : ThingComp
    {
        public CompProperties_InjectableDrugWithConditions Props
        {
            get
            {
                return (CompProperties_InjectableDrugWithConditions)this.props;
            }
        }
        private InjectConditions conditions;
        public string InjectString
        {
            get
            {
                return this.Props.injectCommandString.Formatted(this.parent.LabelShort);
            }
        }
        public override IEnumerable<FloatMenuOption> CompFloatMenuOptions(Pawn selPawn)
        {
            foreach (FloatMenuOption item in base.CompFloatMenuOptions(selPawn))
            {
                yield return item;
            }
            string text = this.InjectString;
            if (this.PawnCanIngest(selPawn, this.parent, out string reason))
            {
                yield return new FloatMenuOption(text, delegate
                {
                    
                    int maxAmountToPickup2 = Math.Min(this.parent.Map.reservationManager.CanReserveStack(selPawn, this.parent, 10, null, false), 1);
                    if (maxAmountToPickup2 == 0)
                    {
                        return;
                    }
                    this.parent.SetForbidden(false, true);
                    Job job = JobMaker.MakeJob(PhoenixLordsJobDefs.OOPhoenixLords_Inject, this.parent);
                    job.count = maxAmountToPickup2;
                    selPawn.jobs.TryTakeOrderedJob(job, new JobTag?(JobTag.Misc), false);
                }, MenuOptionPriority.Default, null, null, 0f, null, null);
            } else
            {
                yield return new FloatMenuOption(text + ": " + reason, null, MenuOptionPriority.Default, null, null, 0f, null, null);
            }
        }
        public IEnumerable<Toil> InjectToils(Pawn pawn)
        {
            List<HediffDef> hediffs = this.Props.appliedHediffs;
            yield break;
        }
        private InjectConditions Conditions
        {
            get
            {
                if (this.conditions == null)
                {
                    this.conditions = (InjectConditions)Activator.CreateInstance(this.Props.injectConditions);
                }
                return this.conditions;
            }
        }
        public bool PawnCanIngest(Pawn pawn, Thing thing, out string reason)
        {
            return this.Conditions.PawnCanIngest(pawn, thing, out reason);
        }
    }
}