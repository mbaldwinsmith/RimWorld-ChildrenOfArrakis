using System;
using System.Reflection;
using HarmonyLib;
using Verse;

namespace ChildrenOfArrakis
{
    /// <summary>
    /// Stores water and manages deathstill processing state.
    /// </summary>
    public class CompDeathstill : CompArrakisWaterStorage
    {
        private int processingTicksRemaining;
        private float waterLeftToProduce;
        private bool processing;

        private static readonly Type MapMeshFlagType = AccessTools.TypeByName("Verse.MapMeshFlag");
        private static readonly MethodInfo MapMeshDirtyMethod = MapMeshFlagType == null
            ? null
            : AccessTools.Method(typeof(MapDrawer), "MapMeshDirty", new[] { typeof(IntVec3), MapMeshFlagType });

        private CompProperties_Deathstill DeathstillProps => (CompProperties_Deathstill)props;

        public bool IsProcessing => processing;
        public string FullTexPath => DeathstillProps?.fullTexPath;

        public bool CanAcceptCorpse()
        {
            return !IsProcessing && DeathstillProps != null && DeathstillProps.waterPerCorpse > 0f && HasSpace(DeathstillProps.waterPerCorpse);
        }

        public bool StartProcessing(Corpse corpse)
        {
            if (corpse == null || DeathstillProps == null || !CanAcceptCorpse())
            {
                return false;
            }

            processing = true;
            processingTicksRemaining = Math.Max(1, DeathstillProps.processingTicks);
            waterLeftToProduce = DeathstillProps.waterPerCorpse;

            corpse.Destroy(DestroyMode.Vanish);
            NotifyGraphicChange();
            return true;
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            if (respawningAfterLoad && processing)
            {
                NotifyGraphicChange();
            }
        }

        public override void CompTick()
        {
            base.CompTick();

            if (!IsProcessing || DeathstillProps == null)
            {
                return;
            }

            float perTick = DeathstillProps.waterPerCorpse / Math.Max(1, DeathstillProps.processingTicks);
            float toAdd = Math.Min(perTick, waterLeftToProduce);

            if (toAdd > 0f && TryAddWater(toAdd))
            {
                waterLeftToProduce -= toAdd;
            }

            processingTicksRemaining--;
            if (processingTicksRemaining <= 0 || waterLeftToProduce <= 0.001f)
            {
                FinishProcessing();
            }
        }

        public override string CompInspectStringExtra()
        {
            string baseString = base.CompInspectStringExtra();

            if (!IsProcessing || DeathstillProps == null)
            {
                return baseString;
            }

            float totalTicks = Math.Max(1, DeathstillProps.processingTicks);
            float progress = 1f - processingTicksRemaining / totalTicks;
            string processingLine = $"Processing corpse: {(progress * 100f):F0}% ({waterLeftToProduce:F1} water left)";

            if (string.IsNullOrEmpty(baseString))
            {
                return processingLine;
            }

            return baseString + "\n" + processingLine;
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref processing, "deathstillProcessing", false);
            Scribe_Values.Look(ref processingTicksRemaining, "deathstillTicksRemaining", 0);
            Scribe_Values.Look(ref waterLeftToProduce, "deathstillWaterLeft", 0f);
        }

        private void FinishProcessing()
        {
            processing = false;
            processingTicksRemaining = 0;
            waterLeftToProduce = 0f;
            NotifyGraphicChange();
        }

        private void NotifyGraphicChange()
        {
            if (!parent.Spawned || parent.Map == null)
            {
                return;
            }

            if (MapMeshFlagType != null && MapMeshDirtyMethod != null)
            {
                try
                {
                    var flag = Enum.Parse(MapMeshFlagType, "Things");
                    MapMeshDirtyMethod.Invoke(parent.Map.mapDrawer, new[] { (object)parent.Position, flag });
                }
                catch
                {
                    // ignore reflection failures; graphic will update on next redraw
                }
            }
        }
    }
}
