using Verse;

namespace TVForPrison
{
    public class CompProperties_TVForPrison : CompProperties
    {
        public float effectFactor;

        public int effectRadius;

        public CompProperties_TVForPrison()
        {
            compClass = typeof(CompTVForPrison);
        }
    }
}