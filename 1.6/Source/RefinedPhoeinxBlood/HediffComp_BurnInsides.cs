using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using UnityEngine;
using Verse;

namespace OOPhoenixLords
{
    public class HediffComp_BurnInsides : HediffComp
    {
        public HediffCompProperties_BurnInsides Props => this.props as HediffCompProperties_BurnInsides;
        public override void CompExposeData()
        {
            base.CompExposeData();
            Scribe_Values.Look<int>(ref this.ticksSinceLastBurn, "ticksSinceLastBurn", 0, false);
        }
        public override void CompPostTickInterval(ref float severityAdjustment, int delta)
        {
            base.CompPostTickInterval(ref severityAdjustment, delta);
            if (this.Pawn.GetStatValue(StatDefOf.ComfyTemperatureMax) >= 999.0)
            {
                return;
            }
            ticksSinceLastBurn += delta;
            int burnCount = Mathf.FloorToInt(ticksSinceLastBurn / (float)this.Props.ticksPerBurnOperation);
            if (burnCount > 0)
            {
                ticksSinceLastBurn -= burnCount * this.Props.ticksPerBurnOperation;
                for (int i = 0; i < burnCount; i++)
                {
                    IEnumerable<BodyPartRecord> hitParts = this.parent.pawn.health.hediffSet.GetNotMissingParts().Where(x => x.coverageAbs > 0f);
                    int burnAmount = this.Props.burnsPerOperation.RandomInRange;
                    for (int j = 0; j < burnAmount; j++)
                    {
                        BodyPartRecord hitPart = hitParts.RandomElementByWeight((BodyPartRecord x) => x.coverageAbs);
                        float amount = this.Props.damagePerBurn;
                        DamageDef damageDef = DamageDefOf.Burn;
                        DamageInfo dinfo = new DamageInfo(
                            def: damageDef,
                            amount: amount,
                            instigator: this.parent.pawn,
                            hitPart: hitPart
                        );
                        dinfo.SetIgnoreArmor(true);
                        this.parent.pawn.TakeDamage(dinfo);
                    }
                }
            }
        }
        private int ticksSinceLastBurn;
    }
}