using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using STS2_ExodiaRelic.STS2_ExodiaRelicCode.Extensions;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.Models.RelicPools;
using STS2_ExodiaRelic.STS2_ExodiaRelicCode.Cards;


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
}