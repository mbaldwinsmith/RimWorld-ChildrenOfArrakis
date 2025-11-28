using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace ChildrenOfArrakis
{
    public class WorkGiver_ProcessDeathstill : WorkGiver_Scanner
    {
        private static readonly ThingDef DeathstillDef = DefDatabase<ThingDef>.GetNamedSilentFail("Arrakis_Deathstill");

        public override PathEndMode PathEndMode => PathEndMode.Touch;

        public override IEnumerable<Thing> PotentialWorkThingsGlobal(Pawn pawn)
        {
            return pawn.Map.listerThings.ThingsInGroup(ThingRequestGroup.Corpse);
        }

        public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            if (DeathstillDef == null || t is not Corpse corpse || corpse.Destroyed)
            {
                return false;
            }

            if (corpse.IsForbidden(pawn) || !pawn.CanReach(corpse, PathEndMode.Touch, Danger.Deadly))
            {
                return false;
            }

            var deathstill = FindAvailableDeathstill(pawn);
            if (deathstill == null)
            {
                return false;
            }

            if (!pawn.CanReserve(corpse))
            {
                return false;
            }

            var comp = deathstill.GetComp<CompDeathstill>();
            return comp != null && comp.CanAcceptCorpse();
        }

        public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            var deathstill = FindAvailableDeathstill(pawn);
            if (deathstill == null)
            {
                return null;
            }

            return JobMaker.MakeJob(DefDatabase<JobDef>.GetNamed("Arrakis_ProcessDeathstill"), deathstill, t);
        }

        private Building FindAvailableDeathstill(Pawn pawn)
        {
            if (DeathstillDef == null)
            {
                return null;
            }

            var list = pawn.Map.listerThings.ThingsOfDef(DeathstillDef);
            Building best = null;
            float bestDist = float.MaxValue;
            foreach (var thing in list)
            {
                if (thing is not Building b || b.IsForbidden(pawn))
                {
                    continue;
                }

                var comp = b.GetComp<CompDeathstill>();
                if (comp == null || !comp.CanAcceptCorpse())
                {
                    continue;
                }

                if (!pawn.CanReserve(b))
                {
                    continue;
                }

                float dist = (pawn.Position - b.Position).LengthHorizontalSquared;
                if (dist < bestDist)
                {
                    bestDist = dist;
                    best = b;
                }
            }

            return best;
        }
    }
}
