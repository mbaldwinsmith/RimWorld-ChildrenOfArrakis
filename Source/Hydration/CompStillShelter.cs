using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using UnityEngine;

namespace ChildrenOfArrakis
{
    /// <summary>
    /// Provides hydration recovery for pawns resting in this bed (stilltent).
    /// </summary>
    public class CompStillShelter : ThingComp
    {
        private CompProperties_StillShelter Props => (CompProperties_StillShelter)props;

        public override void CompTickRare()
        {
            base.CompTickRare();

            if (parent is not Building_Bed bed || parent.Map == null)
            {
                return;
            }

            var occupants = bed.CurOccupants;
            if (occupants == null || !occupants.Any())
            {
                return;
            }

            foreach (var pawn in occupants)
            {
                if (pawn?.needs?.AllNeeds == null)
                {
                    continue;
                }

                var thirst = pawn.needs.AllNeeds.Find(n => n?.def?.defName == Props.thirstDefName);
                if (thirst == null)
                {
                    continue;
                }

                // Only apply while pawn is in bed (asleep or resting).
                thirst.CurLevel = Mathf.Min(thirst.MaxLevel, thirst.CurLevel + Props.thirstRestorePerTickRare);
            }
        }
    }
}
