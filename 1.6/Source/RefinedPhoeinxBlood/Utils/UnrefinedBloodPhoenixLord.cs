using System;
using System.Collections.Generic;
using Verse;
using RimWorld;

namespace OOPhoenixLords
{
    public class UnrefinedBloodPhoenixLord : InjectConditions
    {
        public override bool PawnCanIngest(Pawn pawn, Thing thing, out string reason)
        {
            if (pawn.genes == null)
            {
                reason = "OOPL.NotPhoenixLord".Translate();
                return false;
            }
            Pawn_GeneTracker genes = pawn.genes;
            if (!genes.HasActiveGene(PhoenixLordsGeneDefs.OOPhoenixLords_PhoenixFire))
            {
                reason = "OOPL.NotPhoenixLord".Translate();
                return false;
            }
            if (pawn.health.hediffSet.GetFirstHediffOfDef(PhoenixLordsHediffDefs.OOPhoenixLords_RefinedPhoenixBloodHediff) != null)
            {
                reason = "OOPL.AlreadyInjectedRefinedPhoenixBlood".Translate();
                return false;
            }
            reason = null;
            return true;
        }
    }
}