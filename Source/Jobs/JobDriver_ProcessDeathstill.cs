using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace ChildrenOfArrakis
{
    /// <summary>
    /// Haul a corpse to the deathstill and convert it into stored water.
    /// </summary>
    public class JobDriver_ProcessDeathstill : JobDriver
    {
        public const float DefaultWaterYield = 20f;

        private Building Deathstill => job.GetTarget(TargetIndex.A).Thing as Building;
        private Corpse Corpse => job.GetTarget(TargetIndex.B).Thing as Corpse;
        private CompDeathstill Storage => Deathstill?.GetComp<CompDeathstill>();

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            var ok = pawn.Reserve(job.GetTarget(TargetIndex.A), job, 1, -1, null, errorOnFailed)
                     && pawn.Reserve(job.GetTarget(TargetIndex.B), job, 1, -1, null, errorOnFailed);
            if (ok)
            {
                job.count = 1; // ensure haul count is valid
            }
            return ok;
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
            this.FailOnDespawnedNullOrForbidden(TargetIndex.B);
            this.FailOn(() => Storage == null || Corpse == null);
            this.FailOn(() => !Storage.HasSpace(WaterYield()));

            yield return Toils_Goto.GotoThing(TargetIndex.B, PathEndMode.ClosestTouch);
            yield return Toils_Haul.StartCarryThing(TargetIndex.B);
            yield return Toils_Haul.CarryHauledThingToCell(TargetIndex.A);

            var wait = Toils_General.Wait(600);
            wait.WithProgressBarToilDelay(TargetIndex.A);
            wait.FailOn(() => Storage == null || !Storage.HasSpace(WaterYield()));
            yield return wait;

            yield return new Toil
            {
                initAction = () =>
                {
                    if (Storage == null || Corpse == null)
                    {
                        return;
                    }

                    var yield = WaterYield();
                    if (Storage.TryAddWater(yield))
                    {
                        Corpse.Destroy(DestroyMode.Vanish);
                    }
                },
                defaultCompleteMode = ToilCompleteMode.Instant
            };
        }

        private float WaterYield()
        {
            return Storage?.Props is CompProperties_Deathstill props ? props.waterPerCorpse : DefaultWaterYield;
        }
    }
}
