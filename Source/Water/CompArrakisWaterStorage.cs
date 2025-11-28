using System;
using Verse;

namespace ChildrenOfArrakis
{
    /// <summary>
    /// Simple placeholder water storage for catchbasins until full hydration routing is implemented.
    /// </summary>
    public class CompArrakisWaterStorage : ThingComp
    {
        public float storedWater;

        public CompProperties_ArrakisWaterStorage Props => (CompProperties_ArrakisWaterStorage)props;

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            if (!respawningAfterLoad && storedWater <= 0f)
            {
                storedWater = Props.initialWater;
            }
        }

        public bool HasWater(float amount) => storedWater >= amount;

        public bool HasSpace(float amount) => storedWater + amount <= Props.capacity;

        public bool TryConsume(float amount)
        {
            if (!HasWater(amount))
            {
                return false;
            }

            storedWater -= amount;
            return true;
        }

        public bool TryAddWater(float amount)
        {
            float toAdd = Math.Min(amount, Math.Max(0f, Props.capacity - storedWater));
            if (toAdd <= 0f)
            {
                return false;
            }

            storedWater += toAdd;
            return true;
        }

        public override string CompInspectStringExtra()
        {
            string baseString = base.CompInspectStringExtra();
            string waterLine = $"Stored water: {storedWater:F1} / {Props.capacity:F1}";

            if (Props.sipAmount > 0f)
            {
                waterLine += $" (sip size {Props.sipAmount:F1})";
            }

            if (string.IsNullOrEmpty(baseString))
            {
                return waterLine;
            }

            return baseString + "\n" + waterLine;
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref storedWater, "storedWater", 0f);
        }
    }
}
