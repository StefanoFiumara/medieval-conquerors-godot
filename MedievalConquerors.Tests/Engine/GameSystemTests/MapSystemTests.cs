using FluentAssertions;
using Godot;
using MedievalConquerors.Engine.Actions;
using MedievalConquerors.Engine.Core;
using MedievalConquerors.Engine.Data;
using MedievalConquerors.Engine.Data.Attributes;
using MedievalConquerors.Engine.GameComponents;
using MedievalConquerors.Extensions;
using Xunit.Abstractions;

namespace MedievalConquerors.Tests.Engine.GameSystemTests;

public class MapSystemTests : GameSystemTestFixture
{
    private readonly Player _player;
    private readonly Match _match;

    public MapSystemTests(ITestOutputHelper output) : base(output)
    {
        _player = Game.GetComponent<Match>().LocalPlayer;
        _match = Game.GetComponent<Match>();
        
        // Start the game with the given player
        var action = new BeginGameAction(_player.Id);
        Game.Perform(action);
        Game.Update();
    }

    [Fact]
    public void GameFactory_Creates_MapSystem()
    {
        Game.GetComponent<MapSystem>().Should().NotBeNull();
    }

    [Fact]
    public void MapSystem_Performs_MoveUnitAction_And_Moves_Unit()
    {
        // Play a card
        var card = _player.Hand.First();
        
        var firstPosition = new Vector2I(5, 5);
        var playAction = new PlayCardAction(card, firstPosition);
        Game.Perform(playAction);
        Game.Update();
        
        // set up move attribute
        card.GetAttribute<MovementAttribute>().Distance = 1;
        
        // Then move it
        var newPosition = new Vector2I(5, 4);
        var moveAction = new MoveUnitAction(_player, card, newPosition);
        Game.Perform(moveAction);
        Game.Update();

        card.MapPosition.Should().Be(newPosition);
        Map.GetTile(newPosition).Unit.Should().Be(card);
        Map.GetTile(firstPosition).Unit.Should().BeNull();
        
        card.GetAttribute<MovementAttribute>().RemainingDistance.Should().Be(0);
    }

    [Fact]
    public void MapSystem_MoveUnitAction_Invalidated_Without_MoveAttribute()
    {
        // Play a card
        var card = _player.Hand.First();
        card.Attributes.Remove(typeof(MovementAttribute));
        
        var firstPosition = new Vector2I(5, 5);
        var playAction = new PlayCardAction(card, firstPosition);
        Game.Perform(playAction);
        Game.Update();
        
        // Then attempt to Move it.
        
        var newPosition = new Vector2I(5, 4);
        var moveAction = new MoveUnitAction(_player, card, newPosition);
        Game.Perform(moveAction);
        Game.Update();

        card.MapPosition.Should().Be(firstPosition);
        Map.GetTile(newPosition).Unit.Should().BeNull();
        Map.GetTile(firstPosition).Unit.Should().Be(card);
    }
    
    [Fact]
    public void MapSystem_MoveUnitAction_Invalidated_If_NotEnoughDistance()
    {
        // Play a card
        var card = _player.Hand.First();
        
        var firstPosition = new Vector2I(5, 5);
        var playAction = new PlayCardAction(card, firstPosition);
        Game.Perform(playAction);
        Game.Update();
        
        // set up move attribute
        card.GetAttribute<MovementAttribute>().Distance = 1;
        
        // Then move it
        var newPosition = new Vector2I(5, 3); // 2 tiles away
        var moveAction = new MoveUnitAction(_player, card, newPosition);
        Game.Perform(moveAction);
        Game.Update();

        card.MapPosition.Should().Be(firstPosition);
        Map.GetTile(firstPosition).Unit.Should().Be(card);
        Map.GetTile(newPosition).Unit.Should().BeNull();
    }
    
    [Fact]
    public void MapSystem_MoveUnitAction_Invalidated_If_NotOnMap()
    {
        // Pick a card
        var card = _player.Hand.First();
        
        // set up move attribute
        card.GetAttribute<MovementAttribute>().Distance = 1;
        
        // Then attempt to move it
        var newPosition = new Vector2I(5, 3);
        var moveAction = new MoveUnitAction(_player, card, newPosition);
        Game.Perform(moveAction);
        Game.Update();

        card.MapPosition.Should().Be(HexMap.None);
        Map.GetTile(newPosition).Unit.Should().BeNull();
    }
    
    [Fact]
    public void MapSystem_OnChangeTurn_ResetsAttributes()
    {
        // Play a card
        var card = _player.Hand.First();
        
        var firstPosition = new Vector2I(5, 5);
        var playAction = new PlayCardAction(card, firstPosition);
        Game.Perform(playAction);
        Game.Update();
        
        // set up move attribute
        card.GetAttribute<MovementAttribute>().Distance = 2;
        
        // Then move it
        var newPosition = new Vector2I(5, 3); // 2 tiles away
        var moveAction = new MoveUnitAction(_player, card, newPosition);
        Game.Perform(moveAction);
        Game.Update();

        card.GetAttribute<MovementAttribute>().RemainingDistance.Should().Be(0);
        
        // Change turn to opposite player
        var turnAction = new ChangeTurnAction(_match.OppositePlayer.Id);
        Game.Perform(turnAction);
        Game.Update();
        
        // and back to original player
        turnAction = new ChangeTurnAction(_player.Id);
        Game.Perform(turnAction);
        Game.Update();
        
        card.GetAttribute<MovementAttribute>().RemainingDistance.Should().Be(card.GetAttribute<MovementAttribute>().Distance);
    }
    
    [Fact]
    public void MapSystem_Performs_DiscardCardsAction_And_Removes_From_Map()
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
     
        Map.GetTile(positionToPlay).Unit.Should().BeNull();
        toDiscard.Single().MapPosition.Should().Be(HexMap.None);
    }
}