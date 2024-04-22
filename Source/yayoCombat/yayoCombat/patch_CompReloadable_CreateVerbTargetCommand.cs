using HarmonyLib;
using RimWorld;
using Verse;

namespace yayoCombat;

[HarmonyPatch(typeof(CompApparelVerbOwner), "CreateVerbTargetCommand")]
internal class patch_CompApparelReloadable_CreateVerbTargetCommand
{
    [HarmonyPrefix]
    private static bool Prefix(ref Command_VerbTarget __result, CompApparelVerbOwner __instance, Thing gear, Verb verb)
    {
        if (__instance is CompApparelReloadable value)
        {
            if (!yayoCombat.ammo || !gear.def.IsWeapon)
            {
                return true;
            }

            verb.caster = __instance.Wearer;

            var commandReloadable = new Command_VerbTarget()
            {
                defaultDesc = gear.def.description,
                defaultLabel = verb.verbProps.label,
                verb = verb
            };
            if (verb.verbProps.defaultProjectile != null && verb.verbProps.commandIcon == null)
            {
                commandReloadable.icon = verb.verbProps.defaultProjectile.uiIcon;
                commandReloadable.iconAngle = verb.verbProps.defaultProjectile.uiIconAngle;
                commandReloadable.iconOffset = verb.verbProps.defaultProjectile.uiIconOffset;
                if (verb.verbProps.defaultProjectile.graphicData != null)
                {
                    commandReloadable.defaultIconColor = verb.verbProps.defaultProjectile.graphicData.color;
                }
            }
            else
            {
                commandReloadable.icon = verb.UIIcon != BaseContent.BadTex ? verb.UIIcon : gear.def.uiIcon;
                commandReloadable.iconAngle = gear.def.uiIconAngle;
                commandReloadable.iconOffset = gear.def.uiIconOffset;
                commandReloadable.defaultIconColor = gear.DrawColor;
            }

            if (!__instance.Wearer.IsColonistPlayerControlled || !__instance.Wearer.Drafted)
            {
                commandReloadable.Disable();
            }
            else if (verb.verbProps.violent && __instance.Wearer.WorkTagIsDisabled(WorkTags.Violent))
            {
                commandReloadable.Disable("IsIncapableOfViolenceLower"
                    .Translate(__instance.Wearer.LabelShort, __instance.Wearer).CapitalizeFirst() + ".");
            }
            else if (!__instance.CanBeUsed(out var reason))
            {
                commandReloadable.Disable(value.DisabledReason(value.MinAmmoNeeded(false), value.MaxAmmoNeeded(false)));
            }

            __result = commandReloadable;

            return false;
        }

        return true;
    }
    
}
