using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace ChildrenOfArrakis
{
    public static class FloatMenu_RefillLiterjonPatch
    {
        public static void Apply(Harmony harmony)
        {
            var types3 = new[] { typeof(Vector3), typeof(Pawn), typeof(List<FloatMenuOption>) };
            var types4 = new[] { typeof(Vector3), typeof(Pawn), typeof(List<FloatMenuOption>), typeof(bool) };
            var method = AccessTools.Method(typeof(FloatMenuMakerMap), "AddHumanlikeOrders", types4)
                         ?? AccessTools.Method(typeof(FloatMenuMakerMap), "AddHumanlikeOrders", types3);
            if (method != null)
            {
                harmony.Patch(method, postfix: new HarmonyMethod(typeof(FloatMenu_RefillLiterjonPatch), nameof(Postfix)));
            }
        }

        public static void Postfix(Vector3 clickPos, Pawn pawn, List<FloatMenuOption> opts)
        {
            if (pawn?.Map == null)
            {
                return;
            }

            IntVec3 c = IntVec3.FromVector3(clickPos);
            if (!c.InBounds(pawn.Map))
            {
                return;
            }

            Thing catchbasin = null;
            foreach (var thing in c.GetThingList(pawn.Map))
            {
                if (thing.def?.defName == "Arrakis_Catchbasin")
                {
                    catchbasin = thing;
                    break;
                }
            }

            if (catchbasin == null)
            {
                return;
            }

            var comp = catchbasin.TryGetComp<CompArrakisWaterStorage>();
            if (comp == null)
            {
                return;
            }

            var jobDef = DefDatabase<JobDef>.GetNamedSilentFail("Arrakis_RefillLiterjon");
            if (jobDef == null)
            {
                return;
            }

            string label = "Refill literjon (5 water)";
            if (!pawn.CanReach(catchbasin, PathEndMode.Touch, Danger.Deadly))
            {
                opts.Add(new FloatMenuOption(label + " (no path)", null));
                return;
            }

            if (!pawn.CanReserve(catchbasin))
            {
                opts.Add(new FloatMenuOption(label + " (reserved)", null));
                return;
            }

            if (!comp.HasWater(JobDriver_RefillLiterjon.WaterCost))
            {
                opts.Add(new FloatMenuOption(label + " (not enough water)", null));
                return;
            }

            opts.Add(FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption(label, () =>
            {
                var job = JobMaker.MakeJob(jobDef, catchbasin);
                pawn.jobs.TryTakeOrderedJob(job, JobTag.MiscWork);
            }), pawn, catchbasin));
        }
    }
}
