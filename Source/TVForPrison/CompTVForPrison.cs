using System;
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
        public CompProperties_TVForPrison Props => (CompProperties_TVForPrison) props;

        // Token: 0x17000002 RID: 2
        // (get) Token: 0x06000002 RID: 2 RVA: 0x0000205D File Offset: 0x0000025D
        public Building TV => parent as Building;

        // Token: 0x06000003 RID: 3 RVA: 0x0000206C File Offset: 0x0000026C
        public override void CompTick()
        {
            var tickPeriod = 2500;
            var debug = false;
            if (debug)
            {
                tickPeriod = 120;
            }

            base.CompTick();
            if (Find.TickManager.TicksGame % tickPeriod != 0 || !isFunctional(TV))
            {
                return;
            }

            if (debug)
            {
                Log.Message("TV OK: " + TV.def.label);
            }

            var list = GenRadial.RadialCellsAround(TV.Position, Math.Min(15, Props.effectRadius), true).ToList();
            if (list.Count <= 0)
            {
                return;
            }

            foreach (var cell in list)
            {
                if (!IsValidCell(cell, TV))
                {
                    continue;
                }

                var things = cell.GetThingList(TV.Map);
                if (things.Count <= 0)
                {
                    continue;
                }

                foreach (var thing in things)
                {
                    if (thing is not Pawn pawn)
                    {
                        continue;
                    }

                    if (!IsValidPawn(pawn))
                    {
                        continue;
                    }

                    if (debug)
                    {
                        Log.Message("Pawn OK: " + pawn.Label);
                    }

                    var guest = pawn.guest;
                    var num = guest != null ? new float?(guest.Resistance) : null;
                    var num2 = 0f;
                    if (!((num.GetValueOrDefault() > num2) & (num != null)))
                    {
                        continue;
                    }

                    var res = pawn.guest.Resistance;
                    var factor = 1f;
                    var needs = pawn.needs;

                    if (needs?.mood != null)
                    {
                        factor *= pawn.needs.mood.CurLevel *
                                  Math.Min(1f, Props.effectFactor);
                    }

                    factor *= pawn.health.capacities.GetLevel(PawnCapacityDefOf.Sight);
                    factor *= pawn.health.capacities.GetLevel(PawnCapacityDefOf
                        .Hearing);
                    var tech = TechLevel.Industrial;
                    if (pawn.Faction != null)
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

                    if (!(Math.Min(1f, factor) > 0f))
                    {
                        continue;
                    }

                    var reduction = Math.Min(0.25f, res / 25f * factor);
                    if (res - reduction < 0f)
                    {
                        res = 0f;
                        Messages.Message(
                            "PrisonTV.resbroken".Translate(pawn.LabelShort), MessageTypeDefOf.NeutralEvent);
                    }
                    else
                    {
                        res -= reduction;
                    }

                    pawn.guest.resistance = res;
                    GiveTVThought(pawn);
                }
            }
        }

        // Token: 0x06000004 RID: 4 RVA: 0x00002408 File Offset: 0x00000608
        public void GiveTVThought(Pawn pawn)
        {
            bool hasNeed;
            if (pawn == null)
            {
                hasNeed = false;
            }
            else
            {
                var needs = pawn.needs;
                hasNeed = needs?.mood != null;
            }

            if (!hasNeed)
            {
                return;
            }

            var tvdef = DefDatabase<ThoughtDef>.GetNamed("TVForPrison", false);
            if (tvdef != null)
            {
                pawn.needs.mood.thoughts.memories.TryGainMemory(tvdef);
            }
        }

        // Token: 0x06000005 RID: 5 RVA: 0x0000245C File Offset: 0x0000065C
        public bool IsValidPawn(Pawn pawn)
        {
            return pawn.IsPrisonerInPrisonCell() && !pawn.RaceProps.IsMechanoid && pawn.Awake() && !pawn.Dead &&
                   !pawn.IsBurning() && !pawn.InMentalState && !pawn.Downed;
        }

        // Token: 0x06000006 RID: 6 RVA: 0x000024A9 File Offset: 0x000006A9
        public bool IsValidCell(IntVec3 cell, Building TV)
        {
            return cell.IsValid && cell.InBounds(TV.Map);
        }

        // Token: 0x06000007 RID: 7 RVA: 0x000024C5 File Offset: 0x000006C5

        // Token: 0x06000008 RID: 8 RVA: 0x000024CD File Offset: 0x000006CD
        public bool isFunctional(Building TV)
        {
            return !TV.DestroyedOrNull() && TV?.Map != null && TV.Spawned &&
                   TV.TryGetComp<CompPowerTrader>().PowerOn && !TV.IsBrokenDown();
        }
    }
}