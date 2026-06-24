using System;
using System.Collections.Generic;
using Verse;

namespace OOPhoenixLords
{
    public class HediffComp_PhoenixFlameMultiplier : HediffComp
    {
        public HediffCompProperties_PhoenixFlameMultiplier Props => this.props as HediffCompProperties_PhoenixFlameMultiplier;
        public float Multiplier => this.Props.multiplier;
        public string HediffLabelCap => this.parent.LabelCap;
    }
}