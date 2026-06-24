using System;
using RimWorld;

namespace OOPhoenixLords
{
	public class CompProperties_AbilityPhoenixFireBurst : CompProperties_AbilityEffect
	{
		public CompProperties_AbilityPhoenixFireBurst()
		{
			this.compClass = typeof(CompAbilityEffect_PhoenixFireBurst);
		}

		public float radius = 6f;
		public int damage = 0;
		public bool ignoreLoS = false;
	}
}
