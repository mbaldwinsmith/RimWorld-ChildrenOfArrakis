using RimWorld;
using UnityEngine;
using Verse;

namespace ChildrenOfArrakis
{
    /// <summary>
    /// Simple corpse-type filter tab for the deathstill.
    /// </summary>
    public class ITab_Deathstill : ITab
    {
        private const float Padding = 12f;
        private static readonly Vector2 WindowSize = new(360f, 240f);

        public ITab_Deathstill()
        {
            size = WindowSize;
            labelKey = "TabStorage";
        }

        private CompDeathstill SelectedComp => SelThing?.TryGetComp<CompDeathstill>();

        protected override void FillTab()
        {
            var comp = SelectedComp;
            if (comp == null)
            {
                return;
            }

            Rect rect = new Rect(0f, 0f, WindowSize.x, WindowSize.y).ContractedBy(Padding);
            Listing_Standard listing = new();
            listing.Begin(rect);
            listing.Label("Accepted corpse types:");
            listing.GapLine();

            bool allowHumanlike = comp.AllowHumanlike;
            listing.CheckboxLabeled("Humanlike", ref allowHumanlike, "Allow humanlike corpses.");
            comp.AllowHumanlike = allowHumanlike;

            bool allowAnimals = comp.AllowAnimals;
            listing.CheckboxLabeled("Animals", ref allowAnimals, "Allow animal corpses.");
            comp.AllowAnimals = allowAnimals;

            listing.GapLine();
            bool allowRotten = comp.AllowRotten;
            listing.CheckboxLabeled("Allow rotten corpses", ref allowRotten, "If unchecked, only fresh corpses will be used.");
            comp.AllowRotten = allowRotten;

            listing.End();
        }
    }
}
