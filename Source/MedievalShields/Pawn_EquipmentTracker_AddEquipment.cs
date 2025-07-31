using HarmonyLib;
using RimWorld;
using Verse;

namespace CombatShields;

[HarmonyPatch(typeof(Pawn_EquipmentTracker), nameof(Pawn_EquipmentTracker.AddEquipment))]
public static class Pawn_EquipmentTracker_AddEquipment
{
    public static void Postfix(Pawn_EquipmentTracker __instance, ThingWithComps newEq)
    {
        var owner = __instance.pawn;

        // must have picked up a weapon
        if (!Harmonypatches.PawnHasShieldEquipped(owner) && !Harmonypatches.PawnHasShieldInInventory(owner))
        {
            return;
        }

        if (!newEq.def.IsWeapon)
        {
            return;
        }

        if (Harmonypatches.PawnHasValidEquipped(owner) && Harmonypatches.PawnHasShieldInInventory(owner))
        {
            for (var i = 0; i < owner.inventory.innerContainer.Count; i++)
            {
                if (!Harmonypatches.IsShield(owner.inventory.innerContainer[i].def))
                {
                    continue;
                }

                __instance.pawn.inventory.innerContainer.TryDrop(owner.inventory.innerContainer[i],
                    ThingPlaceMode.Direct, out var whocares);
                owner.apparel.Wear(whocares as Apparel);
            }

            return;
        }

        if (!Harmonypatches.PawnHasShieldEquipped(owner) || Harmonypatches.PawnHasValidEquipped(owner))
        {
            return;
        }

        Apparel shield = null;
        // do we have a shield equipped

        for (var i = 0; i < owner.inventory.innerContainer.Count; i++)
        {
            if (!Harmonypatches.IsShield(owner.inventory.innerContainer[i].def))
            {
                continue;
            }

            __instance.pawn.inventory.innerContainer.TryDrop(
                owner.inventory.innerContainer[i], ThingPlaceMode.Direct, out _);
        }

        for (var i = 0; i < owner.apparel.WornApparelCount; i++)
        {
            if (!Harmonypatches.IsShield(owner.apparel.WornApparel[i].def))
            {
                continue;
            }

            shield = owner.apparel.WornApparel[i];
            break;
        }

        // we have a shield equipped
        if (shield == null)
        {
            return;
        }

        owner.apparel.Remove(shield);
        owner.inventory.innerContainer.TryAddOrTransfer(shield, false);
    }
}