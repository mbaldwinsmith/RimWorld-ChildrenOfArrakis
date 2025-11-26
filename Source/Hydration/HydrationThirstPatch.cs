using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace ChildrenOfArrakis
{
    /// <summary>
    /// Biome-aware tuning for Dubs Bad Hygiene thirst need. Uses runtime reflection to avoid hard dependency.
    /// </summary>
    public static class HydrationThirstPatch
    {
        private const string ThirstDefName = "DubsBadHygiene_Thirst";
        private const int NeedIntervalTicks = 150;

        public static void Apply(Harmony harmony)
        {
            TweakThirstDef();

            var thirstType = AccessTools.TypeByName("DubsBadHygiene.Need_Thirst") ?? AccessTools.TypeByName("Need_Thirst");
            if (thirstType == null)
            {
                return; // Dubs Bad Hygiene not loaded, nothing to patch.
            }

            var needInterval = AccessTools.Method(thirstType, "NeedInterval");
            if (needInterval != null)
            {
                harmony.Patch(needInterval, postfix: new HarmonyMethod(typeof(HydrationThirstPatch), nameof(NeedIntervalPostfix)));
            }
        }

        private static void TweakThirstDef()
        {
            var thirst = DefDatabase<NeedDef>.GetNamedSilentFail(ThirstDefName);
            if (thirst == null)
            {
                return;
            }

            thirst.fallPerDay = 1.6f;
        }

        // Adjust thirst drain per interval by biome dryness.
        public static void NeedIntervalPostfix(Need __instance)
        {
            if (__instance?.def?.defName != ThirstDefName)
            {
                return;
            }

            var pawn = NeedPawn(__instance);
            if (pawn?.Map == null)
            {
                return;
            }

            float fallPerDay = __instance.def?.fallPerDay ?? 0f;
            if (fallPerDay <= 0f)
            {
                return;
            }

            float baseIntervalLoss = fallPerDay / GenDate.TicksPerDay * NeedIntervalTicks;
            float biomeFactor = CalcBiomeFactor(pawn.Map);
            float stillsuitFactor = 1f;
            if (StillsuitUtility.TryGetStillsuit(pawn, out var stillsuit))
            {
                stillsuitFactor = stillsuit.Props.thirstMultiplier;
            }

            float combinedFactor = biomeFactor * stillsuitFactor;

            float delta = baseIntervalLoss * (combinedFactor - 1f);
            if (Mathf.Approximately(delta, 0f))
            {
                return;
            }

            __instance.CurLevel = Mathf.Clamp01(__instance.CurLevel - delta);
        }

        private static float CalcBiomeFactor(Map map)
        {
            var tile = Find.WorldGrid?[map.Tile];
            float rainfall = tile?.rainfall ?? 1000f;
            float dryness = Mathf.Clamp01((2000f - rainfall) / 2000f);
            float factor = 1f + dryness * 0.35f;

            var biomeDefName = map.Biome?.defName;
            if (!string.IsNullOrEmpty(biomeDefName) && (biomeDefName.Contains("Arrakis") || biomeDefName.Contains("DeepDesert")))
            {
                factor = Mathf.Max(factor, 1.35f);
            }

            float temp = map.mapTemperature?.OutdoorTemp ?? 21f;
            if (temp > 40f)
            {
                factor += 0.08f;
            }
            else if (temp < 0f)
            {
                factor -= 0.05f;
            }

            float rainRate = map.weatherManager?.curWeather?.rainRate ?? 0f;
            if (rainRate > 0.1f)
            {
                factor -= 0.05f;
            }

            return Mathf.Clamp(factor, 0.85f, 1.45f);
        }

        private static readonly AccessTools.FieldRef<Need, Pawn> NeedPawn =
            AccessTools.FieldRefAccess<Need, Pawn>("pawn");
    }
}
