using System.Collections.Generic;
using RimWorld;
using Verse;

namespace OOPhoenixLords;

public class Recipe_ExtractPhoenixBlood : Recipe_Surgery
{
    public override bool AvailableOnNow(Thing thing, BodyPartRecord part = null)
    {
      Pawn pawn = thing as Pawn;
      if (pawn?.genes == null || !pawn.genes.HasActiveGene(PhoenixLordsGeneDefs.OOPhoenixLords_PhoenixFire))
      {
        return false;
      }
      if (pawn != null && !pawn.health.CanBleed)
      {
        return false;
      }
      return base.AvailableOnNow(thing, part);
    }
    public override AcceptanceReport AvailableReport(Thing thing, BodyPartRecord part = null)
  {
    return thing is Pawn pawn && pawn.DevelopmentalStage.Baby() ? (AcceptanceReport) "TooSmall".Translate() : base.AvailableReport(thing, part);
  }

  public override bool CompletableEver(Pawn surgeryTarget)
  {
    return base.CompletableEver(surgeryTarget) && this.PawnHasEnoughBloodForExtraction(surgeryTarget);
  }

  public override void CheckForWarnings(Pawn medPawn)
  {
    base.CheckForWarnings(medPawn);
    if (this.PawnHasEnoughBloodForExtraction(medPawn))
      return;
    Messages.Message((string) "OOPL.MessageCannotStartPhoenixBloodExtraction".Translate(medPawn.Named("PAWN")), (LookTargets) (Thing) medPawn, MessageTypeDefOf.NeutralEvent, false);
  }

  public override void ApplyOnPawn(
    Pawn pawn,
    BodyPartRecord part,
    Pawn billDoer,
    List<Thing> ingredients,
    Bill bill)
  {
    if (!this.PawnHasEnoughBloodForExtraction(pawn))
    {
      Messages.Message((string) "OOPL.MessagePawnHadNotEnoughBloodToProducePhoenixBlood".Translate(pawn.Named("PAWN")), (LookTargets) (Thing) pawn, MessageTypeDefOf.NeutralEvent);
    }
    else
    {
      Hediff hediff = HediffMaker.MakeHediff(HediffDefOf.BloodLoss, pawn);
      hediff.Severity = 0.45f;
      pawn.health.AddHediff(hediff);
      this.OnSurgerySuccess(pawn, part, billDoer, ingredients, bill);
      if (!this.IsViolationOnPawn(pawn, part, Faction.OfPlayer))
        return;
      this.ReportViolation(pawn, billDoer, pawn.HomeFaction, -1, PhoenixLordsHistoryEventDefs.OOPhoenixLords_ExtractedPhoenixBlood);
    }
  }

  protected override void OnSurgerySuccess(
    Pawn pawn,
    BodyPartRecord part,
    Pawn billDoer,
    List<Thing> ingredients,
    Bill bill)
  {
    if (GenPlace.TryPlaceThing(ThingMaker.MakeThing(PhoenixLordsThingDefs.OOPhoenixLords_PhoenixBloodSyringe), pawn.PositionHeld, pawn.MapHeld, ThingPlaceMode.Near))
      return;
    Log.Error("Could not drop phoenix blood near " + pawn.PositionHeld.ToString());
  }

  private bool PawnHasEnoughBloodForExtraction(Pawn pawn)
  {
    Hediff firstHediffOfDef = pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.BloodLoss);
    return firstHediffOfDef == null || (double) firstHediffOfDef.Severity < 0.44999998807907104;
  }

}