using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using UnityEngine;
using Verse;


namespace OOPhoenixLords
{
    [StaticConstructorOnStartup]
    public static class DrawUtils
    {
        public static void DrawRingsWithPredicate(Map map, Func<Pawn, bool> predicate = null)
        {
            List<Vector3> ringDrawCells = new List<Vector3>();
            if (map == null)
            {
                return;
            }
            IEnumerable<Pawn> everyPawn = map.mapPawns.AllPawnsSpawned;
            foreach (Pawn item in everyPawn)
            {
                if (predicate == null || predicate(item))
                {
                    ringDrawCells.Add(item.DrawPos);
                }
            }
            foreach (Vector3 c in ringDrawCells)
            {
                DrawTargetHighlightWithLayer(c, AltitudeLayer.MetaOverlays);
            }
        }
        static readonly Material material_again = MaterialPool.MatFrom("UI/Overlays/TargetHighlight_Square", ShaderDatabase.Transparent, Color.green);
        public static void DrawTargetHighlightWithLayer(Vector3 c, AltitudeLayer layer)
        {
            Graphics.DrawMesh(position: new Vector3(c.x, layer.AltitudeFor(), c.z), mesh: MeshPool.plane10, rotation: Quaternion.identity, material: material_again, layer: 0);
        }
    }
}