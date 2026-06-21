using System;
using System.Collections.Generic;
using OOPhoenixLords;
using Verse;
using RimWorld;
using PhoenixRebirth;

namespace OOPhoenixLords
{
	public class Gene_HeatImpervious : GeneWithHediffAndDamageTracking
	{
		public override void PreApplyHediff(Hediff hediff, out bool absorbed)
		{
			base.PreApplyHediff(hediff, out absorbed);
			if (absorbed)
			{
				return;
			}
			absorbed = hediff.def == VanillaDefs.Burn;
		}
		public override void PreApplyDamage(DamageInfo dinfo, out bool absorbed)
		{
			base.PreApplyDamage(dinfo, out absorbed);
			if (absorbed)
			{
				return;
			}
			absorbed = dinfo.Def.armorCategory == VanillaDefs.Heat;
		}
	}
}
