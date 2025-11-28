namespace ChildrenOfArrakis
{
    public class CompProperties_Deathstill : CompProperties_ArrakisWaterStorage
    {
        public float waterPerCorpse = 20f;
        public int processingTicks = 600;
        public string fullTexPath;

        public CompProperties_Deathstill()
        {
            compClass = typeof(CompDeathstill);
        }
    }
}
