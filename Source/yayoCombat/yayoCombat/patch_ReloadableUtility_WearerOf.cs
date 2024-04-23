using HarmonyLib;
using RimWorld;
using RimWorld.Utility;
using Verse;

namespace yayoCombat;

[HarmonyPatch(typeof(ReloadableUtility), "OwnerOf")]
internal class patch_ReloadableUtility_WearerOf
{
    [HarmonyPostfix]
    private static void Postfix(ref Pawn __result, IReloadableComp reloadable)
    {
        if (!yayoCombat.ammo || __result != null)
            return;
        

        if (reloadable is CompApparelReloadable o)
        {
            if(o.ParentHolder is Pawn_EquipmentTracker equipmentTracker && equipmentTracker.pawn != null)
                __result = equipmentTracker.pawn;
        }
        // could also check "is Pawn_InventoryTracker inventoryTracker", might cause problems though?
    }
}
