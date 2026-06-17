using System;
using Verse;
using RimWorld;

namespace OOPhoenixLords
{
	public class ThoughtWorker_ChemfuelSmell : ThoughtWorker
	{
		protected override ThoughtState CurrentSocialStateInternal(Pawn pawn, Pawn other)
		{
			if (!other.RaceProps.Humanlike || !RelationsUtility.PawnsKnowEachOther(pawn, other))
			{
				return false;
			}
			if (!pawn.RaceProps.body.AllParts.Any((BodyPartRecord x) => x.IsInGroup(PhoenixLordsBPGroupDefs.OOPhoenixLords_Nose) && !pawn.health.hediffSet.PartIsMissing(x)))
			{
				return false;
			}
            if (!pawn.genes.HasActiveGene(PhoenixLordsGeneDefs.OOPhoenixLords_ChemfuelSmell) && other.genes.HasActiveGene(PhoenixLordsGeneDefs.OOPhoenixLords_ChemfuelSmell))
            {
                return ThoughtState.ActiveAtStage(0);
            }
			return false;
		}
	}
}
