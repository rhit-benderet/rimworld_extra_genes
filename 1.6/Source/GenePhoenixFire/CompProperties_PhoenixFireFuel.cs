using System;
using System.Collections.Generic;
using Verse;

namespace OOPhoenixLords
{
	public class CompProperties_PhoenixFireFuel : CompProperties
	{
		public CompProperties_PhoenixFireFuel()
		{
			this.compClass = typeof(Comp_PhoenixFireFuel);
		}
        public float refillAmount;
	}
}