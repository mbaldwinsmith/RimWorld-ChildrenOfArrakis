using System;
using HarmonyLib;
using RimWorld;
using Verse;

namespace ChildrenOfArrakis
{
    /// <summary>
    /// Reduces Dubs bladder loss when wearing a stillsuit.
    /// </summary>
    public static class StillsuitBladderPatch
    {
        private const string BladderDefName = "DubsBadHygiene_Bladder";
        private const int NeedIntervalTicks = 150;

        public static void Apply(Harmony harmony)
        {
            var bladderType = AccessTools.TypeByName("DubsBadHygiene.Need_Bladder") ?? AccessTools.TypeByName("Need_Bladder");
            if (bladderType == null)
            {
                return;
            }

            var needInterval = AccessTools.Method(bladderType, "NeedInterval");
            if (needInterval != null)
            {
                harmony.Patch(needInterval, postfix: new HarmonyMethod(typeof(StillsuitBladderPatch), nameof(NeedIntervalPostfix)));
            }
        }

        public static void NeedIntervalPostfix(Need __instance)
        {
            if (__instance?.def?.defName != BladderDefName)
            {
                return;
            }

            var pawn = AccessTools.FieldRefAccess<Need, Pawn>("pawn")(__instance);
            if (pawn == null || !StillsuitUtility.TryGetStillsuit(pawn, out var stillsuit))
            {
                return;
            }

            float fallPerDay = __instance.def?.fallPerDay ?? 0f;
            if (fallPerDay <= 0f)
            {
                return;
            }

            float baseIntervalLoss = fallPerDay / GenDate.TicksPerDay * NeedIntervalTicks;
            float delta = baseIntervalLoss * (stillsuit.Props.bladderMultiplier - 1f);

            __instance.CurLevel = Math.Min(__instance.MaxLevel, __instance.CurLevel - delta);
        }
    }
}
