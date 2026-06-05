using System;
using HarmonyLib;
using UnityEngine;
using Verse;

namespace OOPhoenixLords
{
    public class OOPhoenixLordsMod : Mod
	{
		Settings settings;
        public OOPhoenixLordsMod(ModContentPack content) : base(content)
        {
            this.settings = GetSettings<Settings>();
            var harmony = new Harmony("oophoenixlords");
            harmony.PatchAll();
        }
    }
}