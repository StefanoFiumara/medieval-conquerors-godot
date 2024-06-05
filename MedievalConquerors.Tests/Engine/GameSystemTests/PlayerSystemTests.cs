using FluentAssertions;
using Godot;
using MedievalConquerors.Engine.Actions;
using MedievalConquerors.Engine.Core;
using MedievalConquerors.Engine.Data;
using MedievalConquerors.Engine.GameComponents;
using Xunit.Abstractions;

namespace MedievalConquerors.Tests.Engine.GameSystemTests;

public class PlayerSystemTests : GameSystemTestFixture
{
    private readonly IPlayer _player;
    
    public PlayerSystemTests(ITestOutputHelper output) : base(output)
    {
        _player = Game.GetComponent<Match>().LocalPlayer;
        
        // Start the game with the given player
        var action = new BeginGameAction(_player.Id);
        Game.Perform(action);
        Game.Update();
    }

    [Fact]
    public void GameFactory_Creates_PlayerSystem()
    {
        Game.GetComponent<PlayerSystem>().Should().NotBeNull();
    }
    
    [Fact]
    public void PlayerSystem_Performs_PlayCardAction_And_Moves_To_MapZone()
    {
        // Play a card
        var cardToPlay = _player.Hand.First();
        
        var positionToPlay = new Vector2I(5, 5);
        var playAction = new PlayCardAction(cardToPlay, positionToPlay);
        
        Game.Perform(playAction);
        Game.Update();

        _player.Map.Should().HaveCount(1);
        _player.Hand.Should().HaveCount(5);
        _player.Map.Should().HaveCount(1);
        
        cardToPlay.Zone.Should().Be(Zone.Map);
        cardToPlay.MapPosition.Should().Be(positionToPlay);
        
        var tile = Map.GetTile(positionToPlay);
        
        tile.Unit.Should().NotBeNull();
        tile.Unit.Should().Be(cardToPlay);
    }
    
    [Fact]
    public void PlayerSystem_Performs_DiscardCardsAction_And_Moves_To_DiscardZone()
    {
        // Play a card
        var cardToPlay = _player.Hand.First();
        var positionToPlay = new Vector2I(5, 5);
        var playAction = new PlayCardAction(cardToPlay, positionToPlay);
        Game.Perform(playAction);
        Game.Update();
        
        // Then discard it
        var toDiscard = _player.Map.Take(1).ToList();
        var discardAction = new DiscardCardsAction(toDiscard, _player);

        Game.Perform(discardAction);
        Game.Update();
        
        _player.Map.Should().BeEmpty();
        _player.Discard.Should().HaveCount(1);
        _player.Discard.Should().AllSatisfy(c => c.Zone.Should().Be(Zone.Discard));

        Map.GetTile(positionToPlay).Unit.Should().BeNull();
        toDiscard.Single().MapPosition.Should().Be(HexMap.None);
    }
    
    [Fact]
    public void PlayerSystem_Performs_DiscardCardsAction_And_Moves_Multiple_Cards_To_DiscardZone()
    {
        // Discard 2 random cards
        var toDiscard = _player.Hand.Take(2).ToList();
        var discardAction = new DiscardCardsAction(toDiscard, _player);

        Game.Perform(discardAction);
        Game.Update();
        
        _player.Hand.Should().HaveCount(4);
        _player.Discard.Should().HaveCount(2);
        _player.Discard.Should().AllSatisfy(c => c.Zone.Should().Be(Zone.Discard));
    }
}