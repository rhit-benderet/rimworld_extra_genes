using System;
using System.Collections.Generic;
using OOPhoenixLords;
using Verse;
using RimWorld;
using PhoenixRebirth;

namespace OOPhoenixLords
{
	public abstract class GeneWithHediffAndDamageTracking : Gene
	{
		public virtual void PreApplyHediff(Hediff hediff, out bool absorbed)
		{
			absorbed = false;
		}
		public virtual void PreApplyDamage(DamageInfo dinfo, out bool absorbed)
		{
			absorbed = false;
		}
	}
}
