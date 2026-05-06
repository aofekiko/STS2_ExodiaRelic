using BaseLib.Abstracts;
using BaseLib.Extensions;
using STS2_ExodiaRelic.STS2_ExodiaRelicCode.Extensions;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace STS2_ExodiaRelic.STS2_ExodiaRelicCode.Powers;

public class ExodiaRightLegPower : CustomPowerModel
{
    //Loads from STS2_ExodiaRelic/images/powers/your_power.png
    public override string CustomPackedIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".PowerImagePath();
    public override string CustomBigIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigPowerImagePath();

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        ExodiaRightLegPower exodiaRightLegPower = this;
        if (exodiaRightLegPower.Owner.HasPower<ExodiaHeadPower>() && 
            exodiaRightLegPower.Owner.HasPower<ExodiaLeftArmPower>() &&
            exodiaRightLegPower.Owner.HasPower<ExodiaRightArmPower>() &&
            exodiaRightLegPower.Owner.HasPower<ExodiaLeftLegPower>() &&
            exodiaRightLegPower.Owner.HasPower<ExodiaRightLegPower>())
        {
            foreach (Creature hittableEnemy in (IEnumerable<Creature>) exodiaRightLegPower.CombatState.HittableEnemies)
            {
                NFireBurstVfx child = NFireBurstVfx.Create(hittableEnemy, 5f);
                NCombatRoom instance = NCombatRoom.Instance;
                if (instance != null)
                    instance.CombatVfxContainer.AddChildSafely((Node) child);
            }
            IEnumerable<DamageResult> damageResults = await CreatureCmd.Damage(choiceContext, (IEnumerable<Creature>) exodiaRightLegPower.CombatState.HittableEnemies, (Decimal) 99999999, ValueProp.Unpowered, exodiaRightLegPower.Owner, (CardModel) null);
        }
    }
}