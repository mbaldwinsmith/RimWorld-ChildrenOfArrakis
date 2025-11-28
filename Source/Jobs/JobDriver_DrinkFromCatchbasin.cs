using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

namespace ChildrenOfArrakis
{
    /// <summary>
    /// Simple job: walk to a catchbasin and sip water, restoring DBH thirst if present.
    /// </summary>
    public class JobDriver_DrinkFromCatchbasin : JobDriver
    {
        private const string ThirstDefName = "DubsBadHygiene_Thirst";

        protected Building Catchbasin => (Building)job.targetA.Thing;

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return pawn.Reserve(job.targetA, job, 1, -1, null, errorOnFailed);
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
            this.FailOn(() => StorageComp == null || !StorageComp.HasWater(StorageComp.Props.sipAmount));

            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);

            var drink = Toils_General.Wait(180);
            drink.WithProgressBarToilDelay(TargetIndex.A);
            drink.FailOn(() => StorageComp == null || !StorageComp.HasWater(StorageComp.Props.sipAmount));
            drink.AddFinishAction(DrinkFromStorage);
            yield return drink;
        }

        private CompArrakisWaterStorage StorageComp => Catchbasin?.GetComp<CompArrakisWaterStorage>();

        private void DrinkFromStorage()
        {
            var comp = StorageComp;
            if (comp == null)
            {
                return;
            }

            if (!comp.TryConsume(comp.Props.sipAmount))
            {
                return;
            }

            var thirst = pawn.needs?.AllNeeds?.FirstOrDefault(n => n?.def?.defName == ThirstDefName);
            if (thirst != null)
            {
                float restore = comp.Props.sipAmount * 0.06f; // lean boost to make basins a viable fallback
                thirst.CurLevel = Math.Min(thirst.MaxLevel, thirst.CurLevel + restore);
            }
        }
    }
}
