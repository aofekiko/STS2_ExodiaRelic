using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Events;
using MegaCrit.Sts2.Core.ValueProps;
using STS2_ExodiaRelic.STS2_ExodiaRelicCode.Cards;

namespace STS2_ExodiaRelic.STS2_ExodiaRelicCode.Events;

public class TestEvent() : CustomEventModel()
{
    public override List<(string, string)> Localization => LocManager.Instance.Language switch
    { _ => new EventLoc("Test", 
            new EventPageLoc("INITIAL", "Initial page.",
                new EventOptionLoc("PLAIN", "This", "damage for gold"),
                new EventOptionLoc("ORNATE", "That", "JobApplication for relic")),
            new EventPageLoc("PLAIN", "You picked the boring option."),
            new EventPageLoc("ORNATE", "You picked the wrong option.")
        )
    };

    protected override IReadOnlyList<EventOption> GenerateInitialOptions() =>
    [
        Option(Plain).ThatDoesDamage(DynamicVars.HpLoss.IntValue),
        Option(Ornate, HoverTipFactory.FromCardWithCardHoverTips<JobApplication>())
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new HpLossVar(6),
        new GoldVar(0),
        new StringVar("Curse", ModelDb.Card<JobApplication>().Title)
    ];

    public override void CalculateVars()
    {
        DynamicVars.Gold.BaseValue = Rng.NextInt(41, 69);
    }
    
    public async Task Plain()
    {
        await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), Owner!.Creature, DynamicVars.HpLoss.IntValue, ValueProp.Unblockable | ValueProp.Unpowered, null, null);
        await PlayerCmd.GainGold(DynamicVars.Gold.IntValue, Owner);
        SetEventFinished(PageDescription("PLAIN"));
    }

    public async Task Ornate()
    {
        await RelicCmd.Obtain(RelicFactory.PullNextRelicFromFront(Owner!).ToMutable(), Owner!);
        await CardPileCmd.AddCurseToDeck<JobApplication>(Owner!);
        SetEventFinished(PageDescription("ORNATE"));
    }

    public override string CustomInitialPortraitPath => ImageHelper.GetImagePath($"events/{ModelDb.Event<ThisOrThat>().Id.Entry.ToLowerInvariant()}.png");
    public override string CustomBackgroundScenePath => SceneHelper.GetScenePath("events/background_scenes/" + ModelDb.Event<ThisOrThat>().Id.Entry.ToLowerInvariant());
}
