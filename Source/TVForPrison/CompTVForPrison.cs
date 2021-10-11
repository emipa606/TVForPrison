using RimWorld;
using System;
using System.Linq;
using Verse;

namespace TVForPrison
{
    public class CompTVForPrison : ThingComp
    {
        public CompProperties_TVForPrison Props => (CompProperties_TVForPrison)props;

        public Building TV => parent as Building;
        public static bool IdeoActive = ModsConfig.ActiveModsInLoadOrder.Any(m => m.Name == "Ideology");


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
                    var numw = guest != null ? new float?(guest.will) : null;
                    var num2 = 0f;

                    if (!((num.GetValueOrDefault() > num2) & (num != null)))
                    {
                        continue;
                    }
                    if (!((numw.GetValueOrDefault() > num2) & (numw != null)))
                    {
                        continue;
                    }

                    var res = pawn.guest.Resistance;
                    var will = pawn.guest.will;
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
                    var willreduction = Math.Min(0.25f, will / 25f * factor);

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
                    if (IdeoActive)
                    {
                        if (will - willreduction < 0f)
                        {
                            will = 0f;
                            Messages.Message(
                                "PrisonTV.willbroken".Translate(pawn.LabelShort), MessageTypeDefOf.NeutralEvent);
                        }
                        else
                        {
                            will -= willreduction;
                        }
                    }
                    pawn.guest.resistance = res;
                    if (IdeoActive)
                    {
                        pawn.guest.will = will;
                    }
                    GiveTVThought(pawn);
                }
            }
        }

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

        public bool IsValidPawn(Pawn pawn)
        {
            return pawn.IsPrisonerInPrisonCell() && !pawn.RaceProps.IsMechanoid && pawn.Awake() && !pawn.Dead &&
                   !pawn.IsBurning() && !pawn.InMentalState && !pawn.Downed;
        }

        public bool IsValidCell(IntVec3 cell, Building TV)
        {
            return cell.IsValid && cell.InBounds(TV.Map);
        }


        public bool isFunctional(Building TV)
        {
            return !TV.DestroyedOrNull() && TV?.Map != null && TV.Spawned &&
                   TV.TryGetComp<CompPowerTrader>().PowerOn && !TV.IsBrokenDown();
        }
    }
}
