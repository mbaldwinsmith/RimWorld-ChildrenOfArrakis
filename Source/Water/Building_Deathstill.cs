using UnityEngine;
using Verse;

namespace ChildrenOfArrakis
{
    /// <summary>
    /// Handles stateful visuals for the deathstill (empty vs processing).
    /// </summary>
    public class Building_Deathstill : Building
    {
        private Graphic processingGraphic;

        private CompDeathstill DeathstillComp => GetComp<CompDeathstill>();

        public override Graphic Graphic
        {
            get
            {
                var comp = DeathstillComp;
                if (comp != null && comp.IsProcessing && !comp.FullTexPath.NullOrEmpty())
                {
                    processingGraphic ??= GraphicDatabase.Get<Graphic_Multi>(
                        comp.FullTexPath,
                        def.graphic.Shader,
                        def.graphicData.drawSize,
                        DrawColor);

                    return processingGraphic;
                }

                return base.Graphic;
            }
        }

        public override void DeSpawn(DestroyMode mode = DestroyMode.Vanish)
        {
            base.DeSpawn(mode);
            processingGraphic = null;
        }
    }
}
