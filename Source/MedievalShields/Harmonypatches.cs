using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using Verse;

namespace CombatShields;

[StaticConstructorOnStartup]
internal static class Harmonypatches
{
    public static readonly List<ThingDef> AllRangedWeapons;
    public static readonly List<ThingDef> AllBaseShieldableWeapons;
    public static readonly List<ThingDef> AllBaseLightShieldableWeapons;
    public static readonly List<ThingDef> AllShields;
    public static readonly List<ThingDef> AllLightShields;


    static Harmonypatches()
    {
        new Harmony("ShieldHarmony").PatchAll(Assembly.GetExecutingAssembly());

        AllRangedWeapons = DefDatabase<ThingDef>.AllDefsListForReading
            .Where(def => def.IsRangedWeapon && !def.destroyOnDrop).OrderBy(def => def.label).ToList();
        AllBaseShieldableWeapons =
            AllRangedWeapons.Where(def => def.weaponTags?.Contains("Shield_Sidearm") == true).ToList();
        AllBaseLightShieldableWeapons = AllRangedWeapons
            .Where(def => def.weaponTags?.Contains("LightShield_Sidearm") == true).ToList();
        AllShields = DefDatabase<ThingDef>.AllDefsListForReading.Where(IsShield).OrderBy(def => def.label).ToList();
        AllLightShields = AllShields.Where(def => def.apparel?.tags.Contains("Light_Shield") == true)
            .OrderBy(def => def.label).ToList();

        if (CombatShieldsMod.Instance.Settings.ShieldUse?.Any() == true)
        {
            foreach (var shieldUseKey in CombatShieldsMod.Instance.Settings.ShieldUse.Keys)
            {
                var weapon = DefDatabase<ThingDef>.GetNamedSilentFail(shieldUseKey);
                if (weapon == null)
                {
                    continue;
                }

                if (CombatShieldsMod.Instance.Settings.ShieldUse[shieldUseKey])
                {
                    weapon.weaponTags ??= [];

                    if (weapon.weaponTags.Contains("Shield_Sidearm"))
                    {
                        continue;
                    }

                    weapon.weaponTags.Add("Shield_Sidearm");
                    if (weapon.weaponTags.Contains("LightShield_Sidearm"))
                    {
                        weapon.weaponTags.Remove("LightShield_Sidearm");
                    }

                    continue;
                }

                if (weapon.weaponTags?.Contains("Shield_Sidearm") == true)
                {
                    weapon.weaponTags.Remove("Shield_Sidearm");
                }
            }
        }

        if (CombatShieldsMod.Instance.Settings.LightShieldUse?.Any() != true)
        {
            return;
        }

        foreach (var shieldUseKey in CombatShieldsMod.Instance.Settings.LightShieldUse.Keys)
        {
            var weapon = DefDatabase<ThingDef>.GetNamedSilentFail(shieldUseKey);
            if (weapon == null)
            {
                continue;
            }

            if (CombatShieldsMod.Instance.Settings.LightShieldUse[shieldUseKey])
            {
                weapon.weaponTags ??= [];

                if (weapon.weaponTags.Contains("LightShield_Sidearm"))
                {
                    continue;
                }

                weapon.weaponTags.Add("LightShield_Sidearm");
                if (weapon.weaponTags.Contains("Shield_Sidearm"))
                {
                    weapon.weaponTags.Remove("Shield_Sidearm");
                }

                continue;
            }

            if (weapon.weaponTags?.Contains("LightShield_Sidearm") == true)
            {
                weapon.weaponTags.Remove("LightShield_Sidearm");
            }
        }
    }


// check if a pawn has a shield equipped
    public static bool PawnHasShieldEquipped(Pawn pawn)
    {
        // do we have a shield equipped
        foreach (var apparel in pawn.apparel.WornApparel)
        {
            if (!IsShield(apparel.def))
            {
                continue;
            }

            return true;
        }

        return false;
    }

    // get pawns shield equipped or in inventory
    private static Apparel getPawnShield(Pawn pawn)
    {
        // do we have a shield equipped
        foreach (var apparel in pawn.apparel.WornApparel)
        {
            if (!IsShield(apparel.def))
            {
                continue;
            }

            return apparel;
        }

        foreach (var thing in pawn.inventory.innerContainer)
        {
            if (!IsShield(thing.def))
            {
                continue;
            }

            return thing as Apparel;
        }

        return null;
    }

    // check if a pawn has a shield in inventory
    public static bool PawnHasShieldInInventory(Pawn pawn)
    {
        foreach (var thing in pawn.inventory.innerContainer)
        {
            if (!IsShield(thing.def))
            {
                continue;
            }

            return true;
        }

        return false;
    }


    // check if equiped weapon can be used with shield
    public static bool PawnHasValidEquipped(Pawn pawn)
    {
        // Can always use shield without a weapon
        if (pawn.equipment?.Primary == null)
        {
            return true;
        }

        // If the weapon allows all shields any shield is ok
        if (pawn.equipment.Primary.def.weaponTags.Any(t => t == "Shield_Sidearm"))
        {
            return true;
        }

        // If this is a light sidearm only allow light shield
        if (pawn.equipment.Primary.def.weaponTags.Any(t => t == "LightShield_Sidearm"))
        {
            return getPawnShield(pawn)?.def.apparel.tags.Contains("Light_Shield") == true;
        }

        // Don't allow the remaining ranged weapons
        if (pawn.equipment.Primary.def.IsRangedWeapon)
        {
            return false;
        }

        // Allow all melee unless it cannot be used with a shield
        return !pawn.equipment.Primary.def.weaponTags.Contains("Shield_NoSidearm");
    }

    public static bool IsShield(ThingDef thingDef)
    {
        var returnValue = thingDef.thingClass.IsSubclassOf(typeof(Apparel_Shield)) ||
                          thingDef.thingClass == typeof(Apparel_Shield);

        if (returnValue)
        {
            return true;
        }

        return thingDef.thingCategories != null &&
               thingDef.thingCategories.Any(thingCategoryDef => thingCategoryDef.defName == "Shield");
    }
}