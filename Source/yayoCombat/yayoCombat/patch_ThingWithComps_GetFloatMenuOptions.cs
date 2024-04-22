using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using Verse;

namespace yayoCombat;

[HarmonyPatch(typeof(ThingWithComps), "GetFloatMenuOptions")]
internal class patch_ThingWithComps_GetFloatMenuOptions
{
    [HarmonyPostfix]
    [HarmonyPriority(0)]
    private static void Postfix(ref IEnumerable<FloatMenuOption> __result, ThingWithComps __instance, Pawn selPawn)
    {
        if (yayoCombat.ammo)
        {
            var CompApparelReloadable = __instance.TryGetComp<CompApparelReloadable>();
            if (selPawn.IsColonist && CompApparelReloadable is { AmmoDef: { } } && !CompApparelReloadable.Props.destroyOnEmpty &&
                CompApparelReloadable.RemainingCharges > 0)
            {
                __result = new List<FloatMenuOption>
                {
                    new FloatMenuOption("eject_Ammo".Translate(), cleanWeapon, MenuOptionPriority.High)
                };
            }
        }

        void cleanWeapon()
        {
            reloadUtility.EjectAmmo(selPawn, __instance);
        }
    }
}
