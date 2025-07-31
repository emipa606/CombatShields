using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace CombatShields;

internal class JobDriver_EquipShield : JobDriver
{
    public override bool TryMakePreToilReservations(bool errorOnFailed)
    {
        return pawn.Reserve(job.targetA, job);
    }

    protected override IEnumerable<Toil> MakeNewToils()
    {
        yield return shieldToInventory(pawn);
    }

    private static Toil shieldToInventory(Pawn pawn)
    {
        var toil = new Toil();
        // do we have a shield equipped
        foreach (var apparel in pawn.apparel.WornApparel)
        {
            if (!Harmonypatches.IsShield(apparel.def))
            {
                continue;
            }

            pawn.inventory.innerContainer.TryAddOrTransfer(apparel, false);
        }

        return toil;
    }
}