using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using STS2_ExodiaRelic.STS2_ExodiaRelicCode.Extensions;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;
using STS2_ExodiaRelic.STS2_ExodiaRelicCode.Cards;
using STS2_ExodiaRelic.STS2_ExodiaRelicCode.Powers;


namespace STS2_ExodiaRelic.STS2_ExodiaRelicCode.Relics;

[Pool(typeof(SharedRelicPool))]
public class MilleniumPuzzle : CustomRelicModel
{
    //STS2_ExodiaRelic/images/relics
    public override string PackedIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".RelicImagePath();

    protected override string PackedIconOutlinePath =>
        $"{Id.Entry.RemovePrefix().ToLowerInvariant()}_outline.png".RelicImagePath();

    protected override string BigIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigRelicImagePath();
    
    public override RelicRarity Rarity => RelicRarity.Common;
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips
    {
        get
        {
            IEnumerable<IHoverTip> list = Array.Empty<IHoverTip>();
            list = list.Append(HoverTipFactory.FromCard<ExodiaHead>());
            list = list.Append(HoverTipFactory.FromCard<ExodiaLeftLeg>());
            list = list.Append(HoverTipFactory.FromCard<ExodiaRightLeg>());
            list = list.Append(HoverTipFactory.FromCard<ExodiaLeftArm>());
            list = list.Append(HoverTipFactory.FromCard<ExodiaRightArm>());
            return list;
        }
    }

    public override async Task AfterObtained()
    {
        MilleniumPuzzle milleniumPuzzle = this;
        // ISSUE: object of a compiler-generated type is created
        List<CardPileAddResult> results = new List<CardPileAddResult>();
        results.Add(await CardPileCmd.Add((CardModel) milleniumPuzzle.Owner.RunState.CreateCard<ExodiaHead>(milleniumPuzzle.Owner), PileType.Deck));
        results.Add(await CardPileCmd.Add((CardModel) milleniumPuzzle.Owner.RunState.CreateCard<ExodiaLeftLeg>(milleniumPuzzle.Owner), PileType.Deck));
        results.Add(await CardPileCmd.Add((CardModel) milleniumPuzzle.Owner.RunState.CreateCard<ExodiaRightLeg>(milleniumPuzzle.Owner), PileType.Deck));
        results.Add(await CardPileCmd.Add((CardModel) milleniumPuzzle.Owner.RunState.CreateCard<ExodiaLeftArm>(milleniumPuzzle.Owner), PileType.Deck));
        results.Add(await CardPileCmd.Add((CardModel) milleniumPuzzle.Owner.RunState.CreateCard<ExodiaRightArm>(milleniumPuzzle.Owner), PileType.Deck));
        CardCmd.PreviewCardPileAdd((IReadOnlyList<CardPileAddResult>)results, 2f);
        results = (List<CardPileAddResult>) null;
    }

    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        MilleniumPuzzle milleniumPuzzle = this;
        if (milleniumPuzzle.Owner.HasPower<ExodiaHeadPower>() &&
            milleniumPuzzle.Owner.HasPower<ExodiaLeftArmPower>() &&
            milleniumPuzzle.Owner.HasPower<ExodiaRightArmPower>() &&
            milleniumPuzzle.Owner.HasPower<ExodiaLeftLegPower>() &&
            milleniumPuzzle.Owner.HasPower<ExodiaRightLegPower>()) 
        {

            CombatState combatState = milleniumPuzzle.Owner.Creature.CombatState;
            foreach (Creature hittableEnemy in (IEnumerable<Creature>) combatState.HittableEnemies)
            {
                NFireBurstVfx child = NFireBurstVfx.Create(hittableEnemy, 3f);
                
                NCombatRoom instance = NCombatRoom.Instance;
                if (instance != null)
                    instance.CombatVfxContainer.AddChildSafely((Node) child);
            }
            IEnumerable<DamageResult> damageResults = await CreatureCmd.Damage(choiceContext, (IEnumerable<Creature>) combatState.HittableEnemies , (Decimal) 99999999, ValueProp.Unpowered, milleniumPuzzle.Owner.Creature, (CardModel) null);
        }
    }
}