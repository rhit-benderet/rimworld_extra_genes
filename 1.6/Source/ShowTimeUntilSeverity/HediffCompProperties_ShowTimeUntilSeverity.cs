using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace OOPhoenixLords
{
    public class HediffCompProperties_ShowTimeUntilSeverity : HediffCompProperties
    {
        public HediffCompProperties_ShowTimeUntilSeverity()
        {
            this.compClass = typeof(HediffComp_ShowTimeUntilSeverity);
        }
        public float severity;
        public string label;
    }
}