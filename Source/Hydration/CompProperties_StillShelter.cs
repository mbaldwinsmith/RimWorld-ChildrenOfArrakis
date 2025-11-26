using RimWorld;
using Verse;

namespace ChildrenOfArrakis
{
    public class CompProperties_StillShelter : CompProperties
    {
        public string thirstDefName = "DubsBadHygiene_Thirst";
        public float thirstRestorePerTickRare = 0.015f;

        public CompProperties_StillShelter()
        {
            compClass = typeof(CompStillShelter);
        }
    }
}
