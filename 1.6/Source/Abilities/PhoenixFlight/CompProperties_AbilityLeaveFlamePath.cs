using System;
using Verse;
using RimWorld;

namespace OOPhoenixLords
{
	public class CompProperties_AbilityLeaveFlamePath : CompProperties_AbilityEffect
	{
		public CompProperties_AbilityLeaveFlamePath()
		{
			this.compClass = typeof(CompAbilityEffect_AbilityLeaveFlamePath);
		}
		public float radius;
	}
}
