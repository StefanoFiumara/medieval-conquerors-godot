using MedievalConquerors.Engine.Actions;
using MedievalConquerors.Engine.Data;
using MedievalConquerors.Engine.GameComponents;
using Shouldly;

namespace MedievalConquerors.Tests.Engine.GameSystemTests;

public class HandSystemTests : GameSystemTestFixture
{
    private readonly Player _player;
    
    public HandSystemTests(ITestOutputHelper output, CardLibraryFixture libraryFixture) : base(output, libraryFixture)
    {
        _player = Game.GetComponent<Match>().LocalPlayer;
        
        // Start the game with the given player
        var action = new BeginGameAction(_player.Id);
        Game.Perform(action);
        Game.Update();
    }

    [Fact]
    public void GameFactory_Creates_HandSystem()
    {
        Game.GetComponent<HandSystem>().ShouldNotBeNull();
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    public void HandSystem_Performs_DrawCardsAction_And_Draws_Required_Amount(int amountToDraw)
    {
        var action = new DrawCardsAction(_player.Id, amountToDraw);

        var initialDeckCount = _player.Deck.Count;
        var initialHand = _player.Hand.ToList();
        
        Game.Perform(action);
        Game.Update();

        action.DrawnCards.ShouldBeEquivalentTo(_player.Hand.Except(initialHand));
        
        _player.Hand.Count.ShouldBe(amountToDraw + initialHand.Count);
        _player.Deck.Count.ShouldBe(initialDeckCount - amountToDraw);
        _player.Hand.ShouldAllBe(c => c.Zone == Zone.Hand);
    }
}