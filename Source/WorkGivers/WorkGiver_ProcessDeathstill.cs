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
            return pawn.Map.listerThings.ThingsOfDef(DeathstillDef);
        }

        public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            if (DeathstillDef == null || t is not Building building || building.def != DeathstillDef)
            {
                return false;
            }

            if (building.IsForbidden(pawn) || !pawn.CanReach(building, PathEndMode.Touch, Danger.Deadly))
            {
                return false;
            }

            if (!pawn.CanReserve(building))
            {
                return false;
            }

            var comp = building.GetComp<CompDeathstill>();
            if (comp == null || comp.IsProcessing || !comp.CanAcceptCorpse())
            {
                return false;
            }

            var corpse = FindClosestAllowedCorpse(pawn, comp);
            return corpse != null;
        }

        public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            if (t is not Building building)
            {
                return null;
            }

            var comp = building.GetComp<CompDeathstill>();
            var corpse = comp == null ? null : FindClosestAllowedCorpse(pawn, comp);
            if (corpse == null)
            {
                return null;
            }

            return JobMaker.MakeJob(DefDatabase<JobDef>.GetNamed("Arrakis_ProcessDeathstill"), building, corpse);
        }

        private Corpse FindClosestAllowedCorpse(Pawn pawn, CompDeathstill comp)
        {
            if (comp == null || comp.parent == null || !comp.CanAcceptCorpse())
            {
                return null;
            }

            bool Validator(Thing x)
            {
                if (x is not Corpse corpse || corpse.Destroyed)
                {
                    return false;
                }

                if (corpse.IsForbidden(pawn) || !pawn.CanReserve(corpse))
                {
                    return false;
                }

                return comp.AllowsCorpse(corpse);
            }

            var found = GenClosest.ClosestThingReachable(
                pawn.Position,
                pawn.Map,
                ThingRequest.ForGroup(ThingRequestGroup.Corpse),
                PathEndMode.Touch,
                TraverseParms.For(pawn),
                maxDistance: 9999f,
                validator: Validator);

            return found as Corpse;
        }
    }
}
