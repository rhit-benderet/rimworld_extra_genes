using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using RimWorld;

namespace OOPhoenixLords
{
	public class CompProperties_AbilityPhoenixFlamesCost : CompProperties_AbilityEffect
	{
		public CompProperties_AbilityPhoenixFlamesCost()
		{
			this.compClass = typeof(CompAbilityEffect_AbilityPhoenixFlamesCost);
		}

		public override IEnumerable<string> ExtraStatSummary()
		{
			yield return "OOPL.AbilityPhoenixFlamesCost".Translate() + ": " + Mathf.RoundToInt(this.phoenixFlamesCost * 100f).ToString();
			yield break;
		}

		public float phoenixFlamesCost;
	}
}
