using Verse;

namespace ChildrenOfArrakis
{
    /// <summary>
    /// Marks apparel as a stillsuit and carries hydration/waste tuning.
    /// </summary>
    public class CompStillsuit : ThingComp
    {
        public CompProperties_Stillsuit Props => (CompProperties_Stillsuit)props;
    }
}
