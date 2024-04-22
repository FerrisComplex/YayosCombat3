using System.Reflection;
using HarmonyLib;
using UnityEngine;
using Verse;

namespace yayoCombat;

[HarmonyPatch(typeof(PawnRenderUtility), "DrawEquipmentAiming")]
internal class patch_DrawEquipment
{
    [HarmonyPrefix]
    private static bool Prefix(Thing eq, Vector3 drawLoc, float aimAngle)
    {
        var pawn = eq.ParentHolder as Pawn;
        if (pawn == null) return true;
        
        
        
        
        
        if (!yayoCombat.advAni)
        {
            return true;
        }

        if (yayoCombat.using_meleeAnimations && eq?.def?.IsMeleeWeapon == true)
        {
            return true;
        }

        var rootLoc2 = drawLoc;
        if (pawn.Dead || !pawn.Spawned)
        {
            return false;
        }

        if (eq == null)
        {
            return false;
        }

        if (pawn.CurJob != null && pawn.CurJob.def.neverShowWeapon)
        {
            return false;
        }

        var y = 0.0005f;
        var num = 0f;
        var num2 = 0.1f;
        ThingWithComps thingWithComps = eq as ThingWithComps;
        var stance_Busy = pawn.stances.curStance as Stance_Busy;
        Stance_Busy stance_Busy2 = null;
        var offsetMainHand = Vector3.zero;
        var offsetOffHand = Vector3.zero;
        LocalTargetInfo localTargetInfo = null;
        var num3 = 143f;
        if (stance_Busy is { neverAimWeapon: false })
        {
            localTargetInfo = stance_Busy.focusTarg;
            if (localTargetInfo != null)
            {
                num3 = aimAngle;
            }
        }

        var mainHandAngle = num3;
        var offHandAngle = num3;

        if (pawn.Rotation == Rot4.West)
        {
            y = -0.4787879f;
            num = -0.05f;
        }

        if (pawn.Rotation == Rot4.North)
        {
            y = -0.3787879f;
        }

        if (thingWithComps == null || thingWithComps != pawn.equipment.Primary)
        {
            PawnRenderer_override.animateEquip(pawn.drawer.renderer, pawn, drawLoc + offsetMainHand, mainHandAngle, pawn.equipment.Primary, stance_Busy, new Vector3(0f - num, y, 0f - num2));
        }

        if (thingWithComps != null)
        {
            PawnRenderer_override.animateEquip(
                offset: new Vector3(num, pawn.Rotation == Rot4.East ? -0.42878792f : !(pawn.Rotation == Rot4.West) ? 0f : 0.05f, num2),
                __instance: pawn.drawer.renderer, pawn: pawn, rootLoc: rootLoc2, num: offHandAngle, thing: thingWithComps,
                stance_Busy: stance_Busy2, isSub: true);
        }

        return false;
    }

    private static void SetAnglesAndOffsets(Thing eq, ThingWithComps offHandEquip, float aimAngle, Pawn pawn,
        ref Vector3 offsetMainHand, ref Vector3 offsetOffHand, ref float mainHandAngle, ref float offHandAngle,
        bool mainHandAiming, bool offHandAiming)
    {
    }
}
