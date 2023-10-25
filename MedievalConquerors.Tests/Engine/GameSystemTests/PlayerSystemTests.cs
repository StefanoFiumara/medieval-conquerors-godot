using AutoFixture;
using FluentAssertions;
using MedievalConquerors.Engine.Actions;
using MedievalConquerors.Engine.Core;
using MedievalConquerors.Engine.Data;
using MedievalConquerors.Engine.GameComponents;
using Xunit.Abstractions;

namespace MedievalConquerors.Tests.Engine.GameSystemTests;

public class PlayerSystemTests : GameSystemTestFixture
{
    private readonly PlayerSystem _underTest;
    private readonly IPlayer _player;
    
    public PlayerSystemTests(ITestOutputHelper output) : base(output)
    {
        _underTest = Game.GetComponent<PlayerSystem>();
        
        var dummyCards = Fixture.Build<Card>()
            .FromFactory((ICardData data) => new Card(data, _player, Zone.Deck))
            .CreateMany(30);
        
        _player = Game.GetComponent<Match>().LocalPlayer;
        _player.Deck.AddRange(dummyCards);
    }
    
    [Fact]
    public void PlayerSystem_Performs_DiscardCardsAction_And_Discards_From_Hand()
    {
        // Draw 5 cards
        var drawAction = new DrawCardsAction(5, _player);
        Game.Perform(drawAction);
        Game.Update();
        
        // Then discard 2
        var toDiscard = _player.Hand.Take(2).ToList();
        var discardAction = new DiscardCardsAction(toDiscard, _player);

        Game.Perform(discardAction);
        Game.Update();
        
        _player.Hand.Should().HaveCount(3);
        _player.Discard.Should().HaveCount(2);
        _player.Discard.Should().AllSatisfy(c => c.Zone.Should().Be(Zone.Discard));
    }
    
    [Fact]
    public void PlayerSystem_Performs_DiscardCardsAction_And_Discards_From_Board()
    {
        // Place card on board
        // TODO: use PlayCardAction here when that is implemented
        var cardOnBoard = _player.Deck.First();
        cardOnBoard.Zone = Zone.Board;
        _player.Board.Add(cardOnBoard);
        _player.Deck.Remove(cardOnBoard);
        
        // Then discard it
        var toDiscard = _player.Board.Take(1).ToList();
        var discardAction = new DiscardCardsAction(toDiscard, _player);

        Game.Perform(discardAction);
        Game.Update();
        
        _player.Board.Should().HaveCount(0);
        _player.Discard.Should().HaveCount(1);
        _player.Discard.Should().AllSatisfy(c => c.Zone.Should().Be(Zone.Discard));
    }
}