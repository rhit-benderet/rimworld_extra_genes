using System;
using System.Collections.Generic;
using OOPhoenixLords;
using Verse;
using RimWorld;
using PhoenixRebirth;

namespace OOPhoenixLords
{
	public class Gene_HeatImpervious : GeneWithHediffTracking
	{
		public override void PreApplyHediff(ref Hediff hediff, out bool absorbed)
		{
			base.PreApplyHediff(ref hediff, out absorbed);
			if (absorbed)
			{
				return;
			}
			absorbed = hediff.def == VanillaHedifDefs.Burn;
		}
	}
}
