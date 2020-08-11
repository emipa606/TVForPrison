using System;
using Verse;

namespace TVForPrison
{
	// Token: 0x02000003 RID: 3
	public class CompProperties_TVForPrison : CompProperties
	{
		// Token: 0x0600000A RID: 10 RVA: 0x00002515 File Offset: 0x00000715
		public CompProperties_TVForPrison()
		{
			this.compClass = typeof(CompTVForPrison);
		}

		// Token: 0x04000001 RID: 1
		public int effectRadius;

		// Token: 0x04000002 RID: 2
		public float effectFactor;
	}
}
