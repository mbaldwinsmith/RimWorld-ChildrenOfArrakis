using Verse;

namespace ChildrenOfArrakis
{
    public class CompProperties_ArrakisWaterStorage : CompProperties
    {
        public float capacity = 100f;
        public float initialWater = 40f;
        public float sipAmount = 2.5f;

        public CompProperties_ArrakisWaterStorage()
        {
            compClass = typeof(CompArrakisWaterStorage);
        }
    }
}
