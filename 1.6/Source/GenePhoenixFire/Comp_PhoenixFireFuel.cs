using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace OOPhoenixLords
{
	public class Comp_PhoenixFireFuel : ThingComp
	{
		public CompProperties_PhoenixFireFuel Props
		{
			get
			{
				return (CompProperties_PhoenixFireFuel)this.props;
			}
		}
    }
}