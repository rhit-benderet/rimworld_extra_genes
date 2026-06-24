using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using UnityEngine;
using Verse;

namespace OOPhoenixLords
{
    public class HediffComp_ShowTimeUntilSeverity : HediffComp
    {
        public HediffCompProperties_ShowTimeUntilSeverity Props => this.props as HediffCompProperties_ShowTimeUntilSeverity;
        private HediffComp_SeverityPerDay cachedSeverityPerDay;
        public HediffComp_SeverityPerDay SeverityPerDayComp
        {
            get
            {
                if (this.cachedSeverityPerDay == null)
                {
                    this.cachedSeverityPerDay = this.parent.TryGetComp<HediffComp_SeverityPerDay>();
                }
                return this.cachedSeverityPerDay;
            }
        }
        public float Severity => this.parent.Severity;
        public float SeverityPerDay {
            get
            {
                if (SeverityPerDayComp != null)
                {
                    return SeverityPerDayComp.SeverityChangePerDay();
                }
                return 0;
            }
        }
        public string FormattedLabel {
            get
            {
                if (this.Props.label == null)
                {
                    return null;
                }
                if (this.SeverityPerDay == 0)
                {
                    return null;
                }
                float severityDiff =  this.Props.severity - Severity;
                float ticksToGetToSeverity = severityDiff / this.SeverityPerDay * 60000f;
                if (ticksToGetToSeverity < 0)
                {
                    return null;
                }
                return this.Props.label.Formatted(Mathf.FloorToInt(ticksToGetToSeverity).ToStringTicksToPeriod());
            }
        }
        public override string CompTipStringExtra
		{
			get
			{
				return this.FormattedLabel;
			}
		}
        
    }
}