using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

namespace ChildrenOfArrakis
{
    /// <summary>
    /// Fallback to Arrakis hydration items when Dubs drinking job finds nothing.
    /// </summary>
    public static class HydrationDrinkJobPatch
    {
        public static void Apply(Harmony harmony)
        {
            var jobGiverType = AccessTools.TypeByName("DubsBadHygiene.JobGiver_DrinkWater") ?? AccessTools.TypeByName("JobGiver_DrinkWater");
            if (jobGiverType == null)
            {
                return; // Dubs not present.
            }

            var tryGiveJob = AccessTools.Method(jobGiverType, "TryGiveJob");
            if (tryGiveJob != null)
            {
                harmony.Patch(tryGiveJob, postfix: new HarmonyMethod(typeof(HydrationDrinkJobPatch), nameof(TryGiveJobPostfix)));
            }
        }

        // If DBH returns no job, try to drink a Literjon instead.
        public static void TryGiveJobPostfix(Pawn pawn, ref Job __result)
        {
            if (__result != null || pawn?.Map == null)
            {
                return;
            }

            var basinJob = TryJobFromCatchbasin(pawn);
            if (basinJob != null)
            {
                __result = basinJob;
                return;
            }

            var literjonDef = DefDatabase<ThingDef>.GetNamedSilentFail("Arrakis_Literjon");
            if (literjonDef == null)
            {
                return;
            }

            var literjon = GenClosest.ClosestThingReachable(
                pawn.Position,
                pawn.Map,
                ThingRequest.ForDef(literjonDef),
                PathEndMode.Touch,
                TraverseParms.For(pawn),
                9999f,
                thing => !thing.IsForbidden(pawn) && pawn.CanReserve(thing) && thing.IngestibleNow);

            if (literjon == null)
            {
                return;
            }

            var job = JobMaker.MakeJob(JobDefOf.Ingest, literjon);
            job.count = 1;
            __result = job;
        }

        private static Job TryJobFromCatchbasin(Pawn pawn)
        {
            var basinDef = DefDatabase<ThingDef>.GetNamedSilentFail("Arrakis_Catchbasin");
            var jobDef = DefDatabase<JobDef>.GetNamedSilentFail("Arrakis_DrinkFromCatchbasin");
            if (basinDef == null || jobDef == null)
            {
                return null;
            }

            var basin = GenClosest.ClosestThingReachable(
                pawn.Position,
                pawn.Map,
                ThingRequest.ForDef(basinDef),
                PathEndMode.Touch,
                TraverseParms.For(pawn),
                9999f,
                thing =>
                {
                    if (thing.IsForbidden(pawn) || !pawn.CanReserve(thing))
                    {
                        return false;
                    }
                    var comp = thing.TryGetComp<CompArrakisWaterStorage>();
                    return comp != null && comp.HasWater(comp.Props.sipAmount);
                });

            if (basin == null)
            {
                return null;
            }

            return JobMaker.MakeJob(jobDef, basin);
        }
    }
}
