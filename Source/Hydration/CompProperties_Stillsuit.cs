using Verse;

namespace ChildrenOfArrakis
{
    public class CompProperties_Stillsuit : CompProperties
    {
        /// <summary>
        /// Multiplier to thirst loss; lower values reduce thirst faster.
        /// </summary>
        public float thirstMultiplier = 0.2f;

        /// <summary>
        /// Multiplier to bladder/frequency loss if using DBH bladder need.
        /// </summary>
        public float bladderMultiplier = 0.05f;

        public CompProperties_Stillsuit()
        {
            compClass = typeof(CompStillsuit);
        }
    }
}
