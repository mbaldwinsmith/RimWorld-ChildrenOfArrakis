using HarmonyLib;
using Verse;

namespace ChildrenOfArrakis
{
    /// <summary>
    /// Entry point to apply Harmony patches on load.
    /// </summary>
    public class ChildrenOfArrakisMod : Mod
    {
        private const string HarmonyId = "arrakis.childrenofarrakis";

        public ChildrenOfArrakisMod(ModContentPack content) : base(content)
        {
            var harmony = new Harmony(HarmonyId);
            HydrationThirstPatch.Apply(harmony);
            HydrationDrinkJobPatch.Apply(harmony);
            StillsuitToiletWaterPatch.Apply(harmony);
            StillsuitBladderPatch.Apply(harmony);
            FloatMenu_RefillLiterjonPatch.Apply(harmony);
        }
    }
}
