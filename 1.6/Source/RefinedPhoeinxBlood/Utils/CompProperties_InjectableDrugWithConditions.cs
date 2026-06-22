using System;
using System.Collections.Generic;
using Verse;
using RimWorld;

namespace OOPhoenixLords
{
    public class CompProperties_InjectableDrugWithConditions : CompProperties
    {
        public CompProperties_InjectableDrugWithConditions()
		{
			this.compClass = typeof(Comp_InjectableDrugWithConditions);
		}

        public override IEnumerable<string> ConfigErrors(ThingDef parentDef)
        {
            foreach (string item in base.ConfigErrors(parentDef))
            {
                yield return item;
            }
            if (this.injectConditions == null)
            {
                yield return "injectConditions is null";
            }
        }
        public Type injectConditions = typeof(InjectConditions);
        public EffecterDef injectEffect;

		public SoundDef injectSound;
		public HoldOffsetSet injectHoldOffsetStanding;
		[MustTranslate]
		public string injectCommandString;

		[MustTranslate]
		public string injectReportString;
		public int baseInjectTicks = 500;
		public List<HediffDef> appliedHediffs;
        
    }
}