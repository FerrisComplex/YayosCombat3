using HarmonyLib;
using UnityEngine;
using Verse;

namespace yayoCombat;

[HarmonyPatch(typeof(PawnRenderUtility), "DrawEquipmentAiming")]
public static class patch_DrawEquipmentAiming
{
    [HarmonyPrefix]
    private static bool Prefix(Thing eq, Vector3 drawLoc, float aimAngle)
    {
        if (!yayoCombat.advAni)
        {
            return true;
        }

        if (yayoCombat.using_meleeAnimations && eq.def.IsMeleeWeapon)
        {
            return true;
        }

        var pawn = eq?.holdingOwner?.owner as Pawn;
        if (pawn == null) return true;
        
        var num = aimAngle - 90f;
        var notRanged = false;
        var mirrored = false;

        if (!(pawn.CurJob != null && pawn.CurJob.def.neverShowWeapon) && pawn.stances.curStance is Stance_Busy
            {
                neverAimWeapon: false, focusTarg.IsValid: true
            } stanceBusy)
        {
            if (pawn.Rotation == Rot4.West)
            {
                mirrored = true;
            }

            if (!pawn.equipment.Primary.def.IsRangedWeapon || stanceBusy.verb.IsMeleeAttack)
            {
                notRanged = true;
            }
        }

        Mesh mesh;
        if (notRanged)
        {
            if (mirrored)
            {
                mesh = MeshPool.plane10Flip;
                num -= 180f;
                num -= eq.def.equippedAngleOffset;
            }
            else
            {
                mesh = MeshPool.plane10;
                num += eq.def.equippedAngleOffset;
            }
        }
        else if (aimAngle is > 20f and < 160f)
        {
            mesh = MeshPool.plane10;
            num += eq.def.equippedAngleOffset;
        }
        else if (aimAngle is > 200f and < 340f || mirrored)
        {
            mesh = MeshPool.plane10Flip;
            num -= 180f;
            num -= eq.def.equippedAngleOffset;
        }
        else
        {
            mesh = MeshPool.plane10;
            num += eq.def.equippedAngleOffset;
        }

        num %= 360f;
        var position = PawnRenderer_override.GetDrawOffset(drawLoc, eq, pawn);
        PawnRenderer_override.SaveWeaponLocation(eq, position, aimAngle);

        Graphics.DrawMesh(
            material: eq.Graphic is not Graphic_StackCount graphic_StackCount
                ? eq.Graphic.MatSingle
                : graphic_StackCount.SubGraphicForStackCount(1, eq.def).MatSingle,
            mesh: PawnRenderer_override.GetMesh(mesh, eq, aimAngle, pawn),
            position: position,
            rotation: Quaternion.AngleAxis(num, Vector3.up), layer: 0);
        return false;
    }
}
