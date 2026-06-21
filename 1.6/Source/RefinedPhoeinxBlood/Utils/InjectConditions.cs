using System;
using System.Collections.Generic;
using Verse;
using RimWorld;

namespace OOPhoenixLords
{
    public abstract class InjectConditions
    {
        public abstract bool PawnCanIngest(Pawn pawn, Thing thing, out string reason);

    }
}