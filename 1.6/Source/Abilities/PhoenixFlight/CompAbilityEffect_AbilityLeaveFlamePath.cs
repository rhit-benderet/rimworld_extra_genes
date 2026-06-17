using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace OOPhoenixLords
{
    public class CompAbilityEffect_AbilityLeaveFlamePath : CompAbilityEffect
    {
        private new CompProperties_AbilityLeaveFlamePath Props
		{
			get
			{
				return (CompProperties_AbilityLeaveFlamePath)this.props;
			}
		}
        private Pawn Pawn
		{
			get
			{
				return this.parent.pawn;
			}
		}

		public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
		{
			base.Apply(target, dest);
		}
        public override void DrawEffectPreview(LocalTargetInfo target)
		{
			GenDraw.DrawFieldEdges(this.AffectedCells(target), 2900);
		}
        public void DoExplosion(LocalTargetInfo target, int numOfTicks)
        {
            Pawn pawn = this.Pawn;
            IntVec3 cell = pawn.Position;
			Map mapHeld = pawn.MapHeld;
			float radius = 0f;
			DamageDef flame = DamageDefOf.Flame;
			ThingDef filthDef = PhoenixLordsThingDefs.OOPhoenixLords_Filth_SuperheatedChemfuel;
			int damAmount = 10;
			float armorPenetration = 0f;
			ThingDef postExplosionSpawnThingDef = filthDef;
			float postExplosionSpawnChance = 1f;
			int postExplosionSpawnThingCount = 1;
            float trueRadius = (target.Cell - cell).LengthHorizontal;
            float speed = (trueRadius * 1.5f) / numOfTicks;
            List<IntVec3> overrideCells = this.AffectedCells(target);
            GenExplosion.DoExplosion(
                center: cell,
                map: mapHeld,
                radius: radius,
                damType: flame,
                instigator: pawn,
                damAmount: damAmount,
                armorPenetration: armorPenetration,
                postExplosionSpawnThingDef: postExplosionSpawnThingDef,
                postExplosionSpawnChance: postExplosionSpawnChance,
                postExplosionSpawnThingCount: postExplosionSpawnThingCount,
                chanceToStartFire: 1f,
                overrideCells: overrideCells,
                propagationSpeed: speed
            );
		}
        
        public List<IntVec3> AffectedCells(LocalTargetInfo target)
		{
            Pawn pawn = this.Pawn;
            List<IntVec3> tmpCells = new List<IntVec3>();
			Vector3 sourcePos = pawn.Position.ToVector3().Yto0();
			IntVec3 intVec = target.Cell.ClampInsideMap(pawn.MapHeld);
			if (pawn.Position == intVec)
			{
                int n = GenRadial.NumCellsInRadius(this.Props.radius);
                for (int i = 0; i < n; i++)
                {
                    IntVec3 intVec2 = pawn.Position + GenRadial.RadialPattern[i];
                    if (intVec2.InBounds(pawn.MapHeld) && intVec2.DistanceToSquared(intVec) <= this.Props.radius * this.Props.radius)
                    {
                        tmpCells.Add(intVec2);
                    }
                }
				return tmpCells;
			}
            Vector3 targetPos = intVec.ToVector3().Yto0();
            float minX = Mathf.Min(targetPos.x, sourcePos.x) - this.Props.radius;
            float maxX = Mathf.Max(targetPos.x, sourcePos.x) + this.Props.radius;
            float minZ = Mathf.Min(targetPos.z, sourcePos.z) - this.Props.radius;
            float maxZ = Mathf.Max(targetPos.z, sourcePos.z) + this.Props.radius;
            for (int x = Mathf.FloorToInt(minX); x <= Mathf.CeilToInt(maxX); x++)
            {
                for (int z = Mathf.FloorToInt(minZ); z <= Mathf.CeilToInt(maxZ); z++)
                {
                    Vector3 intVec3 = new Vector3(x, 0, z);
                    if (intVec3.InBounds(pawn.MapHeld) && DistanceSquaredToLine(intVec3, sourcePos, targetPos, minX + this.Props.radius, maxX - this.Props.radius, minZ + this.Props.radius, maxZ - this.Props.radius) < this.Props.radius * this.Props.radius)
                    {
                        tmpCells.Add(intVec3.ToIntVec3());
                    }
                }
            }
			return tmpCells;
		}
        private static float DistanceSquaredToLine(Vector3 point, Vector3 linea, Vector3 lineb, float minX, float maxX, float minZ, float maxZ)
        {
            Vector3 vector = lineb - linea;
            Vector3 orthogonal = new Vector3(vector.z, 0, -vector.x);
            float c = (linea.x * vector.z + vector.x * point.z - point.x * vector.z - vector.x * linea.z) / (vector.x * vector.x + vector.z * vector.z);
            float intersectionPointX = point.x + c * orthogonal.x;
            float intersectionPointZ = point.z + c * orthogonal.z;
            if (intersectionPointX < minX || intersectionPointX > maxX || intersectionPointZ < minZ || intersectionPointZ > maxZ)
            {
                Vector3 toLineA = point - linea;
                Vector3 toLineB = point - lineb;
                float dotA = toLineA.x * toLineA.x + toLineA.z * toLineA.z;
                float dotB = toLineB.x * toLineB.x + toLineB.z * toLineB.z;
                return Mathf.Min(dotA, dotB);
            }
            return c * c * (vector.x * vector.x + vector.z * vector.z);
        }
    }
}