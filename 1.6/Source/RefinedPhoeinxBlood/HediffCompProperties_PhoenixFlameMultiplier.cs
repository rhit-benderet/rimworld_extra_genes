using System;
using System.Collections.Generic;
using Verse;

namespace OOPhoenixLords
{
    public class HediffCompProperties_PhoenixFlameMultiplier : HediffCompProperties
    {
        public HediffCompProperties_PhoenixFlameMultiplier()
        {
            this.compClass = typeof(HediffComp_PhoenixFlameMultiplier);
        }
        public float multiplier;
    }
}