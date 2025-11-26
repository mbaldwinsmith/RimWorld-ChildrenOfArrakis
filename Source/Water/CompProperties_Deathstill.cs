namespace ChildrenOfArrakis
{
    public class CompProperties_Deathstill : CompProperties_ArrakisWaterStorage
    {
        public float waterPerCorpse = 20f;

        public CompProperties_Deathstill()
        {
            compClass = typeof(CompDeathstill);
        }
    }
}
