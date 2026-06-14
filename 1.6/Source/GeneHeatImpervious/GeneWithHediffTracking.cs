using System;
using System.Collections.Generic;
using OOPhoenixLords;
using Verse;
using RimWorld;
using PhoenixRebirth;

namespace OOPhoenixLords
{
	public abstract class GeneWithHediffTracking : Gene
	{
		public virtual void PreApplyHediff(ref Hediff hediff, out bool absorbed)
		{
			absorbed = false;
		}
	}
}
