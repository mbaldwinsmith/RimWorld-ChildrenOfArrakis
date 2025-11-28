using System.Collections.Generic;
using RimWorld;
using Verse;
using UnityEngine;

namespace ChildrenOfArrakis
{
    /// <summary>
    /// Generates small amounts of water and feeds the nearest catchbasin within range.
    /// </summary>
    public class CompWindtrap : ThingComp
    {
        private CompProperties_Windtrap Props => (CompProperties_Windtrap)props;

        public override void CompTickRare()
        {
            base.CompTickRare();

            if (parent.Map == null)
            {
                return;
            }

            if (Props.outdoorsOnly && parent.Position.Roofed(parent.Map))
            {
                return;
            }

            float amount = Props.WaterPerTickRare;
            if (amount <= 0f)
            {
                return;
            }

            amount *= CalcEnvironmentFactor(parent.Map);

            var basin = FindCatchbasinWithSpace();
            basin?.TryGetComp<CompArrakisWaterStorage>()?.TryAddWater(amount);
        }

        public override string CompInspectStringExtra()
        {
            float envFactor = parent?.Map == null ? 1f : CalcEnvironmentFactor(parent.Map);
            float adjustedPerDay = Props.waterPerDay * envFactor;
            string baseLine = $"Water/day: {adjustedPerDay:F1} (env x{envFactor:F2})";
            string targetLine = $"Feeds catchbasins within {Props.outputRadius:F0} tiles";

            return baseLine + "\n" + targetLine;
        }

        private Thing FindCatchbasinWithSpace()
        {
            IEnumerable<IntVec3> cells = GenRadial.RadialCellsAround(parent.Position, Props.outputRadius, true);
            foreach (var cell in cells)
            {
                if (!cell.InBounds(parent.Map))
                {
                    continue;
                }

                List<Thing> things = cell.GetThingList(parent.Map);
                for (int i = 0; i < things.Count; i++)
                {
                    var thing = things[i];
                    if (thing.def?.defName != "Arrakis_Catchbasin")
                    {
                        continue;
                    }

                    var comp = thing.TryGetComp<CompArrakisWaterStorage>();
                    if (comp != null && comp.HasSpace(Props.WaterPerTickRare))
                    {
                        return thing;
                    }
                }
            }

            return null;
        }

        private float CalcEnvironmentFactor(Map map)
        {
            if (map == null)
            {
                return 1f;
            }

            float temp = map.mapTemperature?.OutdoorTemp ?? 21f;
            float tempFactor = Mathf.Lerp(1.05f, 0.55f, Mathf.InverseLerp(-5f, 45f, temp));

            float humidity = map.weatherManager?.curWeather?.rainRate ?? 0f;
            float humidityFactor = 1f + humidity * 0.65f;

            float wind = map.windManager?.WindSpeed ?? 1f;
            float windFactor = Mathf.Clamp01(0.6f + wind * 0.25f);

            return Mathf.Clamp(tempFactor * humidityFactor * windFactor, 0.5f, 1.25f);
        }
    }
}
