using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace ChildrenOfArrakis
{
    /// <summary>
    /// Refill a literjon from a catchbasin by consuming stored water and spawning a new literjon item.
    /// </summary>
    public class JobDriver_RefillLiterjon : JobDriver
    {
        public const float WaterCost = 5f;

        private Building Catchbasin => job.targetA.Thing as Building;

        private CompArrakisWaterStorage StorageComp => Catchbasin?.GetComp<CompArrakisWaterStorage>();

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return pawn.Reserve(job.targetA, job, 1, -1, null, errorOnFailed);
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
            this.FailOn(() => StorageComp == null || !StorageComp.HasWater(WaterCost));

            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);

            var wait = Toils_General.Wait(240);
            wait.WithProgressBarToilDelay(TargetIndex.A);
            wait.FailOn(() => StorageComp == null || !StorageComp.HasWater(WaterCost));
            wait.AddFinishAction(Refill);
            yield return wait;
        }

        private void Refill()
        {
            var comp = StorageComp;
            if (comp == null)
            {
                return;
            }

            if (!comp.TryConsume(WaterCost))
            {
                return;
            }

            var literjonDef = DefDatabase<ThingDef>.GetNamedSilentFail("Arrakis_Literjon");
            if (literjonDef == null)
            {
                return;
            }

            var thing = ThingMaker.MakeThing(literjonDef);
            GenPlace.TryPlaceThing(thing, Catchbasin.Position, Catchbasin.Map, ThingPlaceMode.Near);
        }
    }
}
