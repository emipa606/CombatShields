using HarmonyLib;
using RimWorld;
using Verse;

namespace CombatShields;

[HarmonyPatch(typeof(Pawn_ApparelTracker), nameof(Pawn_ApparelTracker.Wear))]
public static class Pawn_ApparelTracker_Wear
{
    public static void Postfix(Pawn_EquipmentTracker __instance, Apparel newApparel)
    {
        if (!Harmonypatches.IsShield(newApparel.def))
        {
            return;
        }

        var owner = __instance.pawn;

        // must have picked up a weapon
        if (Harmonypatches.PawnHasShieldInInventory(owner))
        {
            // do we have a shield equipped

            for (var i = 0; i < owner.inventory.innerContainer.Count; i++)
            {
                if (!Harmonypatches.IsShield(owner.inventory.innerContainer[i].def))
                {
                    continue;
                }

                __instance.pawn.inventory.innerContainer.TryDrop(owner.inventory.innerContainer[i],
                    ThingPlaceMode.Direct, out _);
            }

            Apparel wornshield = null;

            for (var i = 0; i < owner.apparel.WornApparelCount; i++)
            {
                if (!Harmonypatches.IsShield(owner.apparel.WornApparel[i].def))
                {
                    continue;
                }

                wornshield = owner.apparel.WornApparel[i];
            }

            // we have a shield equipped
            if (wornshield == null)
            {
                return;
            }

            owner.apparel.Remove(wornshield);
            owner.inventory.innerContainer.TryAddOrTransfer(wornshield, false);
        }
        else
        {
            if (Harmonypatches.PawnHasValidEquipped(owner))
            {
                return;
            }

            Apparel wornShield = null;

            for (var i = 0; i < owner.apparel.WornApparelCount; i++)
            {
                if (!Harmonypatches.IsShield(owner.apparel.WornApparel[i].def))
                {
                    continue;
                }

                wornShield = owner.apparel.WornApparel[i];
            }

            // we have a shield equipped
            if (wornShield == null)
            {
                return;
            }

            owner.apparel.Remove(wornShield);
            owner.inventory.innerContainer.TryAddOrTransfer(wornShield, false);
        }
    }
}