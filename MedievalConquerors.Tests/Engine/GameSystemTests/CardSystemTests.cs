using MedievalConquerors.Engine.Actions;
using MedievalConquerors.Engine.Data;
using MedievalConquerors.Engine.Data.Attributes;
using MedievalConquerors.Engine.GameComponents;
using MedievalConquerors.Extensions;
using Shouldly;


namespace MedievalConquerors.Tests.Engine.GameSystemTests;

public class CardSystemTests : GameSystemTestFixture
{
    private readonly Player _player;
    private CardSystem _underTest;

    public CardSystemTests(ITestOutputHelper output, CardLibraryFixture libraryFixture) : base(output, libraryFixture)
    {
        _player = Game.GetComponent<Match>().LocalPlayer;
        _underTest = Game.GetComponent<CardSystem>();
        
        // Start the game with the given player
        var action = new BeginGameAction(_player.Id);
        Game.Perform(action);
        Game.Update();
    }

    [Fact]
    public void GameFactory_Creates_CardSystem()
    {
        Game.GetComponent<CardSystem>().ShouldNotBeNull();
    }

    [Fact]
    public void CardSystem_Marks_Cards_As_Playable()
    {
        var card = _player.Hand.First();
        
        // clear resource costs
        card.GetAttribute<ResourceCostAttribute>().Food = 0;
        card.GetAttribute<ResourceCostAttribute>().Wood = 0;
        card.GetAttribute<ResourceCostAttribute>().Gold = 0;
        card.GetAttribute<ResourceCostAttribute>().Stone = 0;
        
        _underTest.Refresh();
        
        _underTest = Game.GetComponent<CardSystem>();
        _underTest.IsPlayable(card).ShouldBeTrue();
    }

    [Fact]
    public void CardSystem_Marks_Card_Unplayable_When_PlayCondition_Is_Unmet()
    {
        var card = _player.Hand.First();

        card.GetAttribute<ResourceCostAttribute>().Food = 999;
        
        _underTest.Refresh();

        _underTest.IsPlayable(card).ShouldBeFalse();
    }
}