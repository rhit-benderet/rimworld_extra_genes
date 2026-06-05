using System;
using HarmonyLib;
using UnityEngine;
using Verse;

namespace OOPhoenixLords
{
    public class OOPhoenixLordsMod : Mod
	{
        public OOPhoenixLordsMod(ModContentPack content) : base(content)
        {
            var harmony = new Harmony("oophoenixlords");
            harmony.PatchAll();
        }
    }
}