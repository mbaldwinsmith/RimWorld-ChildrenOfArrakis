using System.Collections.Generic;
using RimWorld;
using Verse;

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

            var basin = FindCatchbasinWithSpace();
            basin?.TryGetComp<CompArrakisWaterStorage>()?.TryAddWater(amount);
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
    }
}
