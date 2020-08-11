using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace TVForPrison
{
	// Token: 0x02000002 RID: 2
	public class CompTVForPrison : ThingComp
	{
		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
		public CompProperties_TVForPrison Props
		{
			get
			{
				return (CompProperties_TVForPrison)this.props;
			}
		}

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x06000002 RID: 2 RVA: 0x0000205D File Offset: 0x0000025D
		public Building TV
		{
			get
			{
				return this.parent as Building;
			}
		}

		// Token: 0x06000003 RID: 3 RVA: 0x0000206C File Offset: 0x0000026C
		public override void CompTick()
		{
			int tickPeriod = 2500;
			bool debug = false;
			if (debug)
			{
				tickPeriod = 120;
			}
			base.CompTick();
			if (Find.TickManager.TicksGame % tickPeriod == 0 && this.isFunctional(this.TV))
			{
				if (debug)
				{
					Log.Message("TV OK: " + this.TV.def.label, false);
				}
				List<IntVec3> list = GenRadial.RadialCellsAround(this.TV.Position, (float)Math.Min(15, this.Props.effectRadius), true).ToList<IntVec3>();
				if (list.Count > 0)
				{
					foreach (IntVec3 cell in list)
					{
						if (this.IsValidCell(cell, this.TV))
						{
							List<Thing> things = cell.GetThingList(this.TV.Map);
							if (things.Count > 0)
							{
								foreach (Thing thing in things)
								{
									if (thing is Pawn)
									{
										Pawn pawn = thing as Pawn;
										if (this.IsValidPawn(pawn))
										{
											if (debug)
											{
												Log.Message("Pawn OK: " + pawn.Label, false);
											}
											if (pawn != null)
											{
												Pawn_GuestTracker guest = pawn.guest;
												float? num = (guest != null) ? new float?(guest.Resistance) : null;
												float num2 = 0f;
												if (num.GetValueOrDefault() > num2 & num != null)
												{
													float res = pawn.guest.Resistance;
													float factor = 1f;
													bool flag;
													if (pawn == null)
													{
														flag = (null != null);
													}
													else
													{
														Pawn_NeedsTracker needs = pawn.needs;
														flag = (((needs != null) ? needs.mood : null) != null);
													}
													if (flag)
													{
														factor *= pawn.needs.mood.CurLevel * Math.Min(1f, this.Props.effectFactor);
													}
													factor *= pawn.health.capacities.GetLevel(PawnCapacityDefOf.Sight);
													factor *= pawn.health.capacities.GetLevel(PawnCapacityDefOf.Hearing);
													TechLevel tech = TechLevel.Industrial;
													if (((pawn != null) ? pawn.Faction : null) != null)
													{
														tech = pawn.Faction.def.techLevel;
													}
													switch (tech)
													{
													case TechLevel.Animal:
														factor *= 0.7f;
														break;
													case TechLevel.Neolithic:
														factor *= 0.8f;
														break;
													case TechLevel.Medieval:
														factor *= 0.9f;
														break;
													case TechLevel.Spacer:
														factor *= 0.9f;
														break;
													case TechLevel.Ultra:
														factor *= 0.8f;
														break;
													case TechLevel.Archotech:
														factor *= 0.7f;
														break;
													}
													if (Math.Min(1f, factor) > 0f)
													{
														float reduction = Math.Min(0.25f, res / 25f * factor);
														if (res - reduction < 0f)
														{
															res = 0f;
															Messages.Message("PrisonTV.resbroken".Translate((pawn != null) ? pawn.LabelShort : null), MessageTypeDefOf.NeutralEvent, true);
														}
														else
														{
															res -= reduction;
														}
														pawn.guest.resistance = res;
														this.GiveTVThought(pawn);
													}
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x06000004 RID: 4 RVA: 0x00002408 File Offset: 0x00000608
		public void GiveTVThought(Pawn pawn)
		{
			bool flag;
			if (pawn == null)
			{
				flag = (null != null);
			}
			else
			{
				Pawn_NeedsTracker needs = pawn.needs;
				flag = (((needs != null) ? needs.mood : null) != null);
			}
			if (flag)
			{
				ThoughtDef tvdef = DefDatabase<ThoughtDef>.GetNamed("TVForPrison", false);
				if (tvdef != null)
				{
					pawn.needs.mood.thoughts.memories.TryGainMemory(tvdef, null);
				}
			}
		}

		// Token: 0x06000005 RID: 5 RVA: 0x0000245C File Offset: 0x0000065C
		public bool IsValidPawn(Pawn pawn)
		{
			return pawn.IsPrisonerInPrisonCell() && !pawn.RaceProps.IsMechanoid && pawn.Awake() && !pawn.Dead && !pawn.IsBurning() && !pawn.InMentalState && !pawn.Downed;
		}

		// Token: 0x06000006 RID: 6 RVA: 0x000024A9 File Offset: 0x000006A9
		public bool IsValidCell(IntVec3 cell, Building TV)
		{
			return cell.IsValid && cell.InBounds(TV.Map);
		}

		// Token: 0x06000007 RID: 7 RVA: 0x000024C5 File Offset: 0x000006C5
		public override void CompTickRare()
		{
			base.CompTickRare();
		}

		// Token: 0x06000008 RID: 8 RVA: 0x000024CD File Offset: 0x000006CD
		public bool isFunctional(Building TV)
		{
			return !TV.DestroyedOrNull() && ((TV != null) ? TV.Map : null) != null && TV.Spawned && TV.TryGetComp<CompPowerTrader>().PowerOn && !TV.IsBrokenDown();
		}
	}
}
