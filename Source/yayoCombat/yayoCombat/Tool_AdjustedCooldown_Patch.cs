using HarmonyLib;
using UnityEngine;
using Verse;

namespace yayoCombat;

[HarmonyPatch(typeof(Tool), "AdjustedCooldown", typeof(Thing))]
internal class Tool_AdjustedCooldown_Patch
{
    [HarmonyPostfix]
    [HarmonyPriority(0)]
    private static void Postfix(ref float __result, Thing ownerEquipment)
    {
        if (!yayoCombat.advAni)
        {
            return;
        }

        if (ownerEquipment == null)
        {
            return;
        }

        if (!(__result > 0f))
        {
            return;
        }

        if (ownerEquipment.ParentHolder is not { ParentHolder: Pawn })
        {
            return;
        }

        if (ownerEquipment.def is not { IsMeleeWeapon: true })
        {
            return;
        }

        var num = yayoCombat.meleeDelay * (1f + ((Rand.Value - 0.5f) * yayoCombat.meleeRandom));
        __result = Mathf.Max(__result * num, 0.2f);
    }
}