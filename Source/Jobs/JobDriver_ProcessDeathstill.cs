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
            this.FailOn(() => Storage != null && (!Storage.CanAcceptCorpse() || !Storage.AllowsCorpse(Corpse)));

            yield return Toils_Goto.GotoThing(TargetIndex.B, PathEndMode.ClosestTouch);
            yield return Toils_Haul.StartCarryThing(TargetIndex.B);
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);

            yield return new Toil
            {
                initAction = () =>
                {
                    if (Storage == null || Corpse == null)
                    {
                        EndJobWith(JobCondition.Incompletable);
                        return;
                    }

                    var corpse = Corpse;
                    if (corpse == null || !Storage.AllowsCorpse(corpse) || !Storage.StartProcessing(corpse))
                    {
                        EndJobWith(JobCondition.Incompletable);
                        return;
                    }

                    // ensure the carried thing is cleared if StartProcessing destroyed it
                    if (pawn.carryTracker?.CarriedThing != null && pawn.carryTracker.CarriedThing.Destroyed)
                    {
                        pawn.carryTracker.innerContainer.ClearAndDestroyContents();
                    }
                },
                defaultCompleteMode = ToilCompleteMode.Instant
            };
        }
    }
}
