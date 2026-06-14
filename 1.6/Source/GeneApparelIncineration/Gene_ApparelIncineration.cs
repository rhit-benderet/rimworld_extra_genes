using System;
using System.Collections.Generic;
using OOPhoenixLords;
using Verse;
using RimWorld;
using PhoenixRebirth;
using System.Linq;

namespace OOPhoenixLords
{
	public class Gene_ApparelIncineration : Gene
	{

		public Pawn Pawn
		{
			get
			{
				return this.pawn;
			}
		}

		public string DisplayLabel
		{
			get
			{
				return this.Label + " (" + "Gene".Translate() + ")";
			}
		}
		public IEnumerable<Apparel> GetApparelToIncinerate()
		{
			foreach (Apparel apparel in this.pawn.apparel.WornApparel)
			{
				if (apparel != null && !apparel.Destroyed && apparel.GetStatValue(StatDefOf.ArmorRating_Heat, true) < 0.7f)
				{
					yield return apparel;
				}
			}
			yield break;
		}

		public override void TickInterval(int delta)
		{
			base.TickInterval(delta);
			foreach (Apparel apparel in this.GetApparelToIncinerate().ToList())
			{
				apparel.TakeDamage(new DamageInfo(DamageDefOf.Flame, delta * 5 / 60f, 0f, -1f, this.pawn, null, null, DamageInfo.SourceCategory.ThingOrUnknown, null));
			}
			
		}
	}
}
