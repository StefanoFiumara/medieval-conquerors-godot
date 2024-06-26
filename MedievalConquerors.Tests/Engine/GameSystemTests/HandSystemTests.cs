using FluentAssertions;
using MedievalConquerors.Engine.Actions;
using MedievalConquerors.Engine.Core;
using MedievalConquerors.Engine.Data;
using MedievalConquerors.Engine.GameComponents;
using Xunit.Abstractions;

namespace MedievalConquerors.Tests.Engine.GameSystemTests;

public class HandSystemTests : GameSystemTestFixture
{
    private readonly Player _player;
    
    public HandSystemTests(ITestOutputHelper output) : base(output)
    {
        _player = Game.GetComponent<Match>().LocalPlayer;
    }

    [Fact]
    public void GameFactory_Creates_HandSystem()
    {
        Game.GetComponent<HandSystem>().Should().NotBeNull();
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
        
        Game.Perform(action);
        Game.Update();

        action.DrawnCards.Should().BeEquivalentTo(_player.Hand);
        
        _player.Hand.Should().HaveCount(amountToDraw);
        _player.Deck.Should().HaveCount(initialDeckCount - amountToDraw);
        _player.Hand.Should().AllSatisfy(c => c.Zone.Should().Be(Zone.Hand));
    }
}