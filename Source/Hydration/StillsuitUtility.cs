using System.Linq;
using Verse;

namespace ChildrenOfArrakis
{
    public static class StillsuitUtility
    {
        public static bool TryGetStillsuit(Pawn pawn, out CompStillsuit comp)
        {
            comp = null;
            if (pawn?.apparel?.WornApparel == null)
            {
                return false;
            }

            comp = pawn.apparel.WornApparel
                .SelectMany(apparel => apparel.AllComps ?? Enumerable.Empty<ThingComp>())
                .OfType<CompStillsuit>()
                .FirstOrDefault();

            return comp != null;
        }

        public static bool WearingStillsuit(Pawn pawn) => TryGetStillsuit(pawn, out _);
    }
}
