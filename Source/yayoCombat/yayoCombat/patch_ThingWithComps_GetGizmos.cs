using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using RimWorld;
using Verse;

namespace yayoCombat;

[HarmonyPatch(typeof(Pawn_EquipmentTracker), "GetGizmos")]
internal class patch_ThingWithComps_GetGizmos
{
    [HarmonyPostfix]
    private static IEnumerable<Gizmo> Postfix(IEnumerable<Gizmo> __result, Pawn_EquipmentTracker __instance)
    {
        foreach (var gizmo in __result)
            yield return gizmo;
        if (!yayoCombat.ammo || !PawnAttackGizmoUtility.CanShowEquipmentGizmos()) yield break;
        foreach (var thingWithComps in __instance.AllEquipmentListForReading)
            foreach(var comp in thingWithComps.AllComps)
                if(comp.ParentHolder is Pawn_ApparelTracker)
                    foreach (var gizmo in comp.CompGetWornGizmosExtra())
                            yield return gizmo;
                
            
        
    }
}
