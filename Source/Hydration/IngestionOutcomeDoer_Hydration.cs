using System;
using System.Linq;
using RimWorld;
using Verse;

namespace ChildrenOfArrakis
{
    /// <summary>
    /// Restores the Dubs thirst need when an ingestible is consumed.
    /// </summary>
    public class IngestionOutcomeDoer_Hydration : IngestionOutcomeDoer
    {
        public float thirstRestore = 0.35f;
        public string thirstDefName = "DubsBadHygiene_Thirst";

        protected override void DoIngestionOutcomeSpecial(Pawn pawn, Thing ingested, int ingestedCount)
        {
            if (pawn?.needs?.AllNeeds == null)
            {
                return;
            }

            var thirstNeed = pawn.needs.AllNeeds.FirstOrDefault(n => n?.def?.defName == thirstDefName);
            if (thirstNeed != null)
            {
                thirstNeed.CurLevel = Math.Min(thirstNeed.MaxLevel, thirstNeed.CurLevel + thirstRestore);
            }
        }
    }
}
