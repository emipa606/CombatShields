using System.Collections.Generic;
using Verse;

namespace CombatShields;

/// <summary>
///     Definition of the settings for the mod
/// </summary>
internal class CombatShieldsSettings : ModSettings
{
    public Dictionary<string, bool> LightShieldUse = new();
    private List<string> lightShieldUseKeys;
    private List<bool> lightShieldUseValues;
    public Dictionary<string, bool> ShieldUse = new();
    private List<string> shieldUseKeys;
    private List<bool> shieldUseValues;

    /// <summary>
    ///     Saving and loading the values
    /// </summary>
    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Collections.Look(ref ShieldUse, "ShieldUse", LookMode.Value, LookMode.Value,
            ref shieldUseKeys, ref shieldUseValues);
        Scribe_Collections.Look(ref LightShieldUse, "LightShieldUse", LookMode.Value, LookMode.Value,
            ref lightShieldUseKeys, ref lightShieldUseValues);
    }
}