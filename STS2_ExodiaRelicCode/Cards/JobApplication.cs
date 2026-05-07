using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using STS2_ExodiaRelic.STS2_ExodiaRelicCode.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;

namespace STS2_ExodiaRelic.STS2_ExodiaRelicCode.Cards;


[Pool(typeof(ColorlessCardPool))]
public class JobApplication() :
    CustomCardModel(-1, CardType.Curse, CardRarity.Curse, TargetType.None)
{
    //Image size:
    //Normal art: 1000x760 (Using 500x380 should also work, it will simply be scaled.)
    //Full art: 606x852
    public override string CustomPortraitPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigCardImagePath();

    //Smaller variants of card images for efficiency:
    //Smaller variant of fullart: 250x350
    //Smaller variant of normalart: 250x190

    //Uses card_portraits/card_name.png as image path. These should be smaller images.
    public override string PortraitPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();
    public override string BetaPortraitPath => $"beta/{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();
    
    public override int MaxUpgradeLevel => 0;
    public override bool HasTurnEndInHandEffect => true;
    
    public override IEnumerable<CardKeyword> CanonicalKeywords
    {
        get
        {
            return (IEnumerable<CardKeyword>) new List<CardKeyword>(new CardKeyword[2]
            {
                CardKeyword.Unplayable,
                CardKeyword.Ethereal
            });
        }
    }
    
    public override async Task OnTurnEndInHand(PlayerChoiceContext choiceContext)
    {
        JobApplication jobApplication = this;
        await PlayerCmd.GainGold( 10, jobApplication.Owner);
        bool alreadyHasWeak = jobApplication.Owner.Creature.HasPower<WeakPower>();
        PowerModel powerModel = (PowerModel) await PowerCmd.Apply<WeakPower>(jobApplication.Owner.Creature, 1, (Creature) null, (CardModel) jobApplication);
        if (powerModel == null || alreadyHasWeak)
            return;
        powerModel.SkipNextDurationTick = true;
    }
}