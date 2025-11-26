using System.Linq;
using System.Reflection;
using HarmonyLib;
using Verse;
using Verse.AI;

namespace ChildrenOfArrakis
{
    /// <summary>
    /// When wearing a stillsuit, skip water consumption for Dubs toilet jobs.
    /// Uses reflection so it is safe if Dubs Bad Hygiene is absent or changes method names.
    /// </summary>
    public static class StillsuitToiletWaterPatch
    {
        private static readonly AccessTools.FieldRef<JobDriver, Pawn> JobDriverPawn =
            AccessTools.FieldRefAccess<JobDriver, Pawn>("pawn");

        public static void Apply(Harmony harmony)
        {
            var toiletJobType = AccessTools.TypeByName("DubsBadHygiene.JobDriver_UseToilet");
            if (toiletJobType == null)
            {
                return; // Dubs not present
            }

            foreach (var methodName in new[] { "ConsumeWater", "UseWater", "TryUseWater", "DrawWater" })
            {
                var method = AccessTools.Method(toiletJobType, methodName);
                if (method != null)
                {
                    harmony.Patch(method, prefix: new HarmonyMethod(typeof(StillsuitToiletWaterPatch), nameof(SkipWaterIfStillsuit)));
                    break;
                }
            }
        }

        // If wearing a stillsuit, zero out any numeric water-use parameters.
        public static void SkipWaterIfStillsuit(object __instance, object[] __args)
        {
            var pawn = (__instance as JobDriver) != null ? JobDriverPawn((JobDriver)__instance) : null;
            if (!StillsuitUtility.WearingStillsuit(pawn))
            {
                return;
            }

            for (int i = 0; i < __args.Length; i++)
            {
                switch (__args[i])
                {
                    case int _:
                        __args[i] = 0;
                        break;
                    case float _:
                        __args[i] = 0f;
                        break;
                    case double _:
                        __args[i] = 0d;
                        break;
                }
            }
        }
    }
}
