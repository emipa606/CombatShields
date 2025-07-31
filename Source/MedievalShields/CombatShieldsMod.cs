using System.Collections.Generic;
using System.Linq;
using Mlie;
using UnityEngine;
using Verse;

namespace CombatShields;

[StaticConstructorOnStartup]
internal class CombatShieldsMod : Mod
{
    /// <summary>
    ///     The instance of the settings to be read by the mod
    /// </summary>
    public static CombatShieldsMod Instance;

    private static string currentVersion;
    private static readonly Vector2 searchSize = new(200f, 25f);
    private static readonly Vector2 iconSize = new(58f, 58f);
    private static string searchText = "";
    private static Vector2 scrollPosition;
    private static readonly Color alternateBackground = new(0.2f, 0.2f, 0.2f, 0.5f);

    /// <summary>
    ///     Constructor
    /// </summary>
    /// <param name="content"></param>
    public CombatShieldsMod(ModContentPack content) : base(content)
    {
        Instance = this;
        Settings = GetSettings<CombatShieldsSettings>();
        currentVersion = VersionFromManifest.GetVersionFromModMetaData(content.ModMetaData);
        Settings.ShieldUse ??= new Dictionary<string, bool>();

        Settings.LightShieldUse ??= new Dictionary<string, bool>();
    }

    /// <summary>
    ///     The instance-settings for the mod
    /// </summary>
    internal CombatShieldsSettings Settings { get; }

    /// <summary>
    ///     The title for the mod-settings
    /// </summary>
    /// <returns></returns>
    public override string SettingsCategory()
    {
        return "Combat Shields";
    }

    /// <summary>
    ///     The settings-window
    ///     For more info: https://rimworldwiki.com/wiki/Modding_Tutorials/ModSettings
    /// </summary>
    /// <param name="rect"></param>
    public override void DoSettingsWindowContents(Rect rect)
    {
        var listingStandard = new Listing_Standard();
        listingStandard.Begin(rect);

        if (currentVersion != null)
        {
            listingStandard.Gap();
            GUI.contentColor = Color.gray;
            listingStandard.Label("CombatShields.modVersion".Translate(currentVersion));
            GUI.contentColor = Color.white;
        }

        var searchRect = listingStandard.GetRect(25f);
        var resetRect = searchRect;
        resetRect.width /= 8;
        if (Widgets.ButtonText(resetRect, "CombatShields.reset".Translate()))
        {
            reset();
        }

        searchRect.width /= 2;
        searchText = Widgets.TextEntryLabeled(searchRect, "CombatShields.search".Translate(), searchText);
        TooltipHandler.TipRegion(searchRect, "CombatShields.searchtt".Translate());

        Text.Font = GameFont.Tiny;

        Widgets.Label(new Rect(
            searchRect.position +
            new Vector2(rect.width - (iconSize.x * 3.8f), searchRect.height / 2f),
            searchSize), "CombatShields.usablewith".Translate());

        var shieldsLabelRect = new Rect(
            searchRect.position +
            new Vector2(rect.width - (iconSize.x * 2.4f), searchRect.height / 2f),
            new Vector2(iconSize.x * 1.4f, 25f));
        var lightShieldsLabelRect = new Rect(
            searchRect.position +
            new Vector2(rect.width - (iconSize.x * 1.4f), searchRect.height / 2f),
            new Vector2(iconSize.x * 1.4f, 25f));
        Widgets.Label(shieldsLabelRect, "CombatShields.usewithshields".Translate());
        Widgets.Label(lightShieldsLabelRect, "CombatShields.usewithlightshields".Translate());
        TooltipHandler.TipRegion(shieldsLabelRect,
            string.Join("\n", Harmonypatches.AllShields.Select(def => def.LabelCap)));
        TooltipHandler.TipRegion(lightShieldsLabelRect,
            string.Join("\n", Harmonypatches.AllLightShields.Select(def => def.LabelCap)));

        listingStandard.GapLine();
        listingStandard.End();

        var borderRect = rect;

        borderRect.y += listingStandard.CurHeight;
        borderRect.height -= listingStandard.CurHeight;

        var allRangedWeapons = Harmonypatches.AllRangedWeapons;
        if (!string.IsNullOrEmpty(searchText))
        {
            allRangedWeapons = allRangedWeapons.Where(def =>
                def.label.ToLower().Contains(searchText.ToLower()) ||
                def.modContentPack?.Name.ToLower().Contains(searchText.ToLower()) == true).ToList();
        }

        var scrollContentRect = rect;
        scrollContentRect.height = allRangedWeapons.Count * 61f;
        scrollContentRect.width -= 20;
        scrollContentRect.x = 0;
        scrollContentRect.y = 0;


        var scrollListing = new Listing_Standard();
        Widgets.BeginScrollView(borderRect, ref scrollPosition, scrollContentRect);
        scrollListing.Begin(scrollContentRect);
        var alternate = false;
        foreach (var rangedWeapon in allRangedWeapons)
        {
            var modInfo = rangedWeapon.modContentPack?.Name;
            var rowRect = scrollListing.GetRect(60);
            alternate = !alternate;
            if (alternate)
            {
                Widgets.DrawBoxSolid(rowRect.ExpandedBy(10, 0), alternateBackground);
            }

            var weaponLabel = $"{rangedWeapon.label.CapitalizeFirst()} ({rangedWeapon.defName}) - {modInfo}";
            drawIcon(rangedWeapon, new Rect(rowRect.position, iconSize));
            var nameRect = new Rect(rowRect.position + new Vector2(iconSize.x, 0),
                rowRect.size - new Vector2(iconSize.x * 2, 0));
            rangedWeapon.weaponTags ??= [];

            var canBeUsed = rangedWeapon.weaponTags.Contains("Shield_Sidearm");
            var couldBeUsed = canBeUsed;
            var canBeUsedLight = rangedWeapon.weaponTags.Contains("LightShield_Sidearm");
            var couldBeUsedLight = canBeUsedLight;

            Widgets.Label(nameRect, weaponLabel);
            Widgets.Checkbox(rowRect.position + new Vector2(rowRect.width, 0) - new Vector2(iconSize.x * 2, 0),
                ref canBeUsed);
            Widgets.Checkbox(rowRect.position + new Vector2(rowRect.width, 0) - new Vector2(iconSize.x, 0),
                ref canBeUsedLight);


            if (canBeUsed == couldBeUsed && canBeUsedLight == couldBeUsedLight)
            {
                continue;
            }

            if (canBeUsed != couldBeUsed)
            {
                if (canBeUsed)
                {
                    rangedWeapon.weaponTags.Add("Shield_Sidearm");
                    if (canBeUsedLight)
                    {
                        rangedWeapon.weaponTags.Remove("LightShield_Sidearm");
                    }
                }
                else
                {
                    rangedWeapon.weaponTags.Remove("Shield_Sidearm");
                }

                updateWeapon(rangedWeapon);
                continue;
            }

            if (canBeUsedLight)
            {
                rangedWeapon.weaponTags.Add("LightShield_Sidearm");
                if (canBeUsed)
                {
                    rangedWeapon.weaponTags.Remove("Shield_Sidearm");
                }
            }
            else
            {
                rangedWeapon.weaponTags.Remove("LightShield_Sidearm");
            }

            updateWeapon(rangedWeapon);
        }

        scrollListing.End();
        Widgets.EndScrollView();
    }

    private void updateWeapon(ThingDef weaponDef)
    {
        if (weaponDef.weaponTags?.Contains("Shield_Sidearm") == true)
        {
            if (Harmonypatches.AllBaseShieldableWeapons.Contains(weaponDef))
            {
                Settings.ShieldUse.Remove(weaponDef.defName);
            }
            else
            {
                Settings.ShieldUse[weaponDef.defName] = true;
            }
        }
        else
        {
            if (Harmonypatches.AllBaseShieldableWeapons.Contains(weaponDef))
            {
                Settings.ShieldUse[weaponDef.defName] = false;
            }
            else
            {
                Settings.ShieldUse.Remove(weaponDef.defName);
            }
        }

        if (weaponDef.weaponTags?.Contains("LightShield_Sidearm") == true)
        {
            if (Harmonypatches.AllBaseLightShieldableWeapons.Contains(weaponDef))
            {
                Settings.LightShieldUse.Remove(weaponDef.defName);
            }
            else
            {
                Settings.LightShieldUse[weaponDef.defName] = true;
            }
        }
        else
        {
            if (Harmonypatches.AllBaseLightShieldableWeapons.Contains(weaponDef))
            {
                Settings.LightShieldUse[weaponDef.defName] = false;
            }
            else
            {
                Settings.LightShieldUse.Remove(weaponDef.defName);
            }
        }
    }

    private void reset()
    {
        foreach (var rangedWeapon in Harmonypatches.AllRangedWeapons)
        {
            rangedWeapon.weaponTags ??= [];

            if (Harmonypatches.AllBaseShieldableWeapons.Contains(rangedWeapon))
            {
                if (!rangedWeapon.weaponTags.Contains("Shield_Sidearm"))
                {
                    rangedWeapon.weaponTags.Add("Shield_Sidearm");
                }
            }
            else
            {
                if (rangedWeapon.weaponTags.Contains("Shield_Sidearm"))
                {
                    rangedWeapon.weaponTags.Remove("Shield_Sidearm");
                }
            }

            if (Harmonypatches.AllBaseLightShieldableWeapons.Contains(rangedWeapon))
            {
                if (!rangedWeapon.weaponTags.Contains("LightShield_Sidearm"))
                {
                    rangedWeapon.weaponTags.Add("LightShield_Sidearm");
                }
            }
            else
            {
                if (rangedWeapon.weaponTags.Contains("LightShield_Sidearm"))
                {
                    rangedWeapon.weaponTags.Remove("LightShield_Sidearm");
                }
            }
        }

        Settings.ShieldUse = new Dictionary<string, bool>();
        Settings.LightShieldUse = new Dictionary<string, bool>();
    }


    private static void drawIcon(ThingDef rangedWeapon, Rect rect)
    {
        var texture2D = Widgets.GetIconFor(rangedWeapon);

        if (texture2D == null)
        {
            return;
        }

        if (texture2D.width != texture2D.height)
        {
            var ratio = (float)texture2D.width / texture2D.height;

            if (ratio < 1)
            {
                rect.x += (rect.width - (rect.width * ratio)) / 2;
                rect.width *= ratio;
            }
            else
            {
                rect.y += (rect.height - (rect.height / ratio)) / 2;
                rect.height /= ratio;
            }
        }

        GUI.DrawTexture(rect, texture2D);
        TooltipHandler.TipRegion(rect, rangedWeapon.LabelCap);
    }
}