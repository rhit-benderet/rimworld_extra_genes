using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace OOPhoenixLords
{
    public class HediffCompProperties_BurnInsides : HediffCompProperties
    {
        public HediffCompProperties_BurnInsides()
        {
            this.compClass = typeof(HediffComp_BurnInsides);
        }
        public float damagePerBurn;
        public int ticksPerBurnOperation;
        public IntRange burnsPerOperation;
    }
}