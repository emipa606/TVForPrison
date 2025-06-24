using System;
using System.Linq;
using RimWorld;
using Verse;

namespace TVForPrison;

public class CompTVForPrison : ThingComp
{
    private static readonly bool ideoActive = ModLister.IdeologyInstalled;
    private CompProperties_TVForPrison Props => (CompProperties_TVForPrison)props;

    private Building TV => parent as Building;


    public override void CompTick()
    {
        const int tickPeriod = 2500;
#if DEBUG
            tickPeriod = 120;
#endif

        base.CompTick();
        if (Find.TickManager.TicksGame % tickPeriod != 0 || !isFunctional(TV))
        {
            return;
        }

#if DEBUG
            Log.Message("TV OK: " + TV.def.label);
#endif

        var list = GenRadial.RadialCellsAround(TV.Position, Math.Min(15, Props.effectRadius), true).ToList();
        if (list.Count <= 0)
        {
            return;
        }

        foreach (var cell in list)
        {
            if (!isValidCell(cell, TV))
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

                if (!isValidPawn(pawn))
                {
                    continue;
                }
#if DEBUG
                    Log.Message("Pawn OK: " + pawn.Label);
#endif

                var guest = pawn.guest;

                if (guest == null)
                {
                    continue;
                }

                var guestResistance = guest.Resistance;
                var guestWill = guest.will;
                const float lowestValue = 0f;

                if (guestResistance <= lowestValue && guestWill <= lowestValue)
                {
                    continue;
                }

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

                var reduction = Math.Min(0.25f, guestResistance / 25f * factor);
                var willReduction = Math.Min(0.25f, guestWill / 25f * factor);

                if (guestResistance > lowestValue)
                {
                    if (guestResistance - reduction < 0f)
                    {
                        guestResistance = lowestValue;
                        Messages.Message(
                            "PrisonTV.resbroken".Translate(pawn.LabelShort), MessageTypeDefOf.NeutralEvent);
                    }
                    else
                    {
                        guestResistance -= reduction;
                    }
                }

                if (ideoActive && guestWill > lowestValue)
                {
                    if (guestWill - willReduction < 0f)
                    {
                        guestWill = 0f;
                        Messages.Message(
                            "PrisonTV.willbroken".Translate(pawn.LabelShort), MessageTypeDefOf.NeutralEvent);
                    }
                    else
                    {
                        guestWill -= willReduction;
                    }
                }

                pawn.guest.resistance = guestResistance;
                if (ideoActive)
                {
                    pawn.guest.will = guestWill;
                }

                giveTVThought(pawn);
            }
        }
    }

    private static void giveTVThought(Pawn pawn)
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

        var tvDef = DefDatabase<ThoughtDef>.GetNamed("TVForPrison", false);
        if (tvDef != null)
        {
            pawn.needs.mood.thoughts.memories.TryGainMemory(tvDef);
        }
    }

    private static bool isValidPawn(Pawn pawn)
    {
        return pawn.IsPrisonerInPrisonCell() && !pawn.RaceProps.IsMechanoid && pawn.Awake() && !pawn.Dead &&
               !pawn.IsBurning() && !pawn.InMentalState && !pawn.Downed;
    }

    private static bool isValidCell(IntVec3 cell, Building tv)
    {
        return cell.IsValid && cell.InBounds(tv.Map);
    }


    private static bool isFunctional(Building tv)
    {
        return !tv.DestroyedOrNull() && tv is { Map: not null, Spawned: true } &&
               tv.TryGetComp<CompPowerTrader>().PowerOn && !tv.IsBrokenDown();
    }
}