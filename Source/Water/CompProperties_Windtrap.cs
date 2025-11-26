using Verse;

namespace ChildrenOfArrakis
{
    public class CompProperties_Windtrap : CompProperties
    {
        public float waterPerDay = 6f;
        public float outputRadius = 8f;
        public bool outdoorsOnly = true;

        public CompProperties_Windtrap()
        {
            compClass = typeof(CompWindtrap);
        }

        public float WaterPerTickRare => waterPerDay / (RimWorld.GenDate.TicksPerDay / 250f);
    }
}
