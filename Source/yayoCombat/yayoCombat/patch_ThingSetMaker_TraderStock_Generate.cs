using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace yayoCombat;

[HarmonyPatch(typeof(ThingSetMaker_TraderStock), "Generate")]
internal class patch_ThingSetMaker_TraderStock_Generate
{
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        CodeInstruction returnInstruction = null;
        // Original code
        foreach (var instruction in instructions)
        {
            if (instruction.opcode == OpCodes.Ret)
            {
                returnInstruction = instruction;
            }
            else
            {
                yield return instruction;
            }
        }

        // New code
        yield return new CodeInstruction(OpCodes.Ldloc_0);
        yield return new CodeInstruction(OpCodes.Ldloc_1);
        yield return new CodeInstruction(OpCodes.Ldloc_2);
        yield return new CodeInstruction(OpCodes.Ldarg_2);
        yield return new CodeInstruction(OpCodes.Call,
            typeof(patch_ThingSetMaker_TraderStock_Generate).GetMethod(
                nameof(AddAmmo),
                BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic));
        yield return returnInstruction;
    }

    private static void AddAmmo(TraderKindDef traderKindDef, Faction makingFaction, int forTile, List<Thing> outThings)
    {
        var tradeTagFieldInfo =
            typeof(StockGenerator_Tag).GetField("TradeTag", BindingFlags.NonPublic | BindingFlags.Instance);
        var isWeaponsTrader = traderKindDef.stockGenerators.FirstOrDefault(s => s is StockGenerator_WeaponsRanged) !=
                              null;
        var isExoticTrader = !isWeaponsTrader &&
                             traderKindDef.stockGenerators.FirstOrDefault(s => s is StockGenerator_Tag tag &&
                                                                               (string)tradeTagFieldInfo
                                                                                   ?.GetValue(tag) == "ExoticMisc") !=
                             null;
        if (!traderKindDef.defName.ToLower().Contains("bulkgoods")
            && !isWeaponsTrader
            && !isExoticTrader
            && !(Rand.Value <= 0.33f))
        {
            return;
        }

        var tech = TechLevel.Spacer;
        if (makingFaction?.def != null)
        {
            tech = makingFaction.def.techLevel;
        }

        if (tech < TechLevel.Neolithic)
        {
            return;
        }

        var amount = 400f;
        var min = 0.25f;
        var max = 1.50f;

        Thing thing;

        // ?? ??
        var rnd = Rand.Value;
        int count;
        if (!isExoticTrader
            && (tech <= TechLevel.Medieval // 100% for Neolithic & Medieval
                || tech <= TechLevel.Industrial && rnd <= 0.2f //  20% for Industrial
                || rnd <= 0.1f)) //  10% for Post-Industrial
        {
            var primitiveAmount = amount;
            if (tech > TechLevel.Medieval)
            {
                primitiveAmount *= 0.5f;
            }

            count = Mathf.RoundToInt(Rand.Range(min, max) * yayoCombat.ammoGen * primitiveAmount);
            if (count > 20)
            {
                thing = ThingMaker.MakeThing(ThingDef.Named("yy_ammo_primitive"));
                thing.stackCount = count;
                thing.PostGeneratedForTrader(traderKindDef, forTile, makingFaction);
                outThings.Add(thing);
            }

            count = Mathf.RoundToInt(Rand.Range(min, max) * yayoCombat.ammoGen * primitiveAmount * 0.40f);
            if (count > 20)
            {
                thing = ThingMaker.MakeThing(ThingDef.Named("yy_ammo_primitive_fire"));
                thing.stackCount = count;
                thing.PostGeneratedForTrader(traderKindDef, forTile, makingFaction);
                outThings.Add(thing);
            }

            count = Mathf.RoundToInt(Rand.Range(min, max) * yayoCombat.ammoGen * primitiveAmount * 0.25f);
            if (count > 20)
            {
                thing = ThingMaker.MakeThing(ThingDef.Named("yy_ammo_primitive_emp"));
                thing.stackCount = count;
                thing.PostGeneratedForTrader(traderKindDef, forTile, makingFaction);
                outThings.Add(thing);
            }
        }


        // ?? ??
        rnd = Rand.Value;
        if (!isExoticTrader
            && (tech == TechLevel.Industrial // 100% for Industrial
                || tech > TechLevel.Industrial && rnd <= 0.8f //  80% for Post-Industrial
                || rnd <= 0.2f)) //  20% for Pre-Industrial
        {
            var industrialAmount = amount;
            if (tech < TechLevel.Industrial)
            {
                industrialAmount /= (int)TechLevel.Industrial - (int)tech;
            }

            count = Mathf.RoundToInt(Rand.Range(min, max) * yayoCombat.ammoGen * industrialAmount);
            if (count > 20)
            {
                thing = ThingMaker.MakeThing(ThingDef.Named("yy_ammo_industrial"));
                thing.stackCount = count;
                thing.PostGeneratedForTrader(traderKindDef, forTile, makingFaction);
                outThings.Add(thing);
            }

            count = Mathf.RoundToInt(Rand.Range(min, max) * yayoCombat.ammoGen * industrialAmount * 0.40f);
            if (count > 20)
            {
                thing = ThingMaker.MakeThing(ThingDef.Named("yy_ammo_industrial_fire"));
                thing.stackCount = count;
                thing.PostGeneratedForTrader(traderKindDef, forTile, makingFaction);
                outThings.Add(thing);
            }

            count = Mathf.RoundToInt(Rand.Range(min, max) * yayoCombat.ammoGen * industrialAmount * 0.25f);
            if (count > 20)
            {
                thing = ThingMaker.MakeThing(ThingDef.Named("yy_ammo_industrial_emp"));
                thing.stackCount = count;
                thing.PostGeneratedForTrader(traderKindDef, forTile, makingFaction);
                outThings.Add(thing);
            }
        }


        // ?? ??
        rnd = Rand.Value;
        if (!isExoticTrader
            && tech < TechLevel.Spacer && ( // 100% for Spacer & Post-Spacer
                tech != TechLevel.Industrial || !(rnd <= 0.5f)) && !( //  50% for Industrial
                rnd <= 0.1f)) //  10% for Pre-Industrial
        {
            return;
        }

        var spacerAmount = amount;
        if (tech < TechLevel.Spacer)
        {
            spacerAmount /= (int)TechLevel.Spacer - (int)tech;
        }

        count = Mathf.RoundToInt(Rand.Range(min, max) * yayoCombat.ammoGen * spacerAmount);
        if (count > 20)
        {
            thing = ThingMaker.MakeThing(ThingDef.Named("yy_ammo_spacer"));
            thing.stackCount = count;
            thing.PostGeneratedForTrader(traderKindDef, forTile, makingFaction);
            outThings.Add(thing);
        }

        count = Mathf.RoundToInt(Rand.Range(min, max) * yayoCombat.ammoGen * spacerAmount * 0.40f);
        if (count > 20)
        {
            thing = ThingMaker.MakeThing(ThingDef.Named("yy_ammo_spacer_fire"));
            thing.stackCount = count;
            thing.PostGeneratedForTrader(traderKindDef, forTile, makingFaction);
            outThings.Add(thing);
        }

        count = Mathf.RoundToInt(Rand.Range(min, max) * yayoCombat.ammoGen * spacerAmount * 0.25f);
        if (count <= 20)
        {
            return;
        }

        thing = ThingMaker.MakeThing(ThingDef.Named("yy_ammo_spacer_emp"));
        thing.stackCount = count;
        thing.PostGeneratedForTrader(traderKindDef, forTile, makingFaction);
        outThings.Add(thing);
    }
}