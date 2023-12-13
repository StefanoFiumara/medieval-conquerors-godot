using FluentAssertions;
using Godot;
using MedievalConquerors.Engine.Actions;
using MedievalConquerors.Engine.Core;
using MedievalConquerors.Engine.Data;
using MedievalConquerors.Engine.Data.Attributes;
using MedievalConquerors.Engine.GameComponents;
using Xunit.Abstractions;

namespace MedievalConquerors.Tests.Engine.GameSystemTests;

public class BoardSystemTests : GameSystemTestFixture
{
    private readonly IPlayer _player;

    public BoardSystemTests(ITestOutputHelper output) : base(output)
    {
        _player = Game.GetComponent<Match>().LocalPlayer;
    }

    [Fact]
    public void GameFactory_Creates_BoardSystem()
    {
        Game.GetComponent<BoardSystem>().Should().NotBeNull();
    }

    [Fact]
    public void BoardSystem_Performs_MoveUnitAction_And_Moves_Unit()
    {
        // Draw some cards
        var drawAction = new DrawCardsAction(_player.Id, 5);
        Game.Perform(drawAction);
        Game.Update();
        
        // Then play one
        var card = _player.Hand.First();
        
        var firstPosition = new Vector2I(5, 5);
        var playAction = new PlayCardAction(_player, card, firstPosition);
        Game.Perform(playAction);
        Game.Update();
        
        // set up move attribute
        var moveAttribute = new MoveAttribute(1);
        card.CardData.Attributes.Add(moveAttribute);
        card.Attributes.Add(moveAttribute);
        
        // Then move it
        var newPosition = new Vector2I(5, 4);
        var moveAction = new MoveUnitAction(_player, card, newPosition);
        Game.Perform(moveAction);
        Game.Update();

        card.BoardPosition.Should().Be(newPosition);
        Board.GetTile(newPosition).Objects.Should().Contain(card);
        Board.GetTile(firstPosition).Objects.Should().BeEmpty();
        
        moveAttribute.DistanceRemaining.Should().Be(0);
    }

    [Fact]
    public void BoardSystem_MoveUnitAction_Invalidated_Without_MoveAttribute()
    {
        // Draw some cards
        var drawAction = new DrawCardsAction(_player.Id, 5);
        Game.Perform(drawAction);
        Game.Update();
        
        // Then play one
        var card = _player.Hand.First();
        
        var firstPosition = new Vector2I(5, 5);
        var playAction = new PlayCardAction(_player, card, firstPosition);
        Game.Perform(playAction);
        Game.Update();
        
        // Then attempt to Move it.
        var newPosition = new Vector2I(5, 4);
        var moveAction = new MoveUnitAction(_player, card, newPosition);
        Game.Perform(moveAction);
        Game.Update();

        card.BoardPosition.Should().Be(firstPosition);
        Board.GetTile(newPosition).Objects.Should().BeEmpty();
        Board.GetTile(firstPosition).Objects.Should().Contain(card);
    }
    
    [Fact]
    public void BoardSystem_MoveUnitAction_Invalidated_If_NotEnoughDistance()
    {
        // Draw some cards
        var drawAction = new DrawCardsAction(_player.Id, 5);
        Game.Perform(drawAction);
        Game.Update();
        
        // Then play one
        var card = _player.Hand.First();
        
        var firstPosition = new Vector2I(5, 5);
        var playAction = new PlayCardAction(_player, card, firstPosition);
        Game.Perform(playAction);
        Game.Update();
        
        // set up move attribute
        var moveAttribute = new MoveAttribute(1);
        card.CardData.Attributes.Add(moveAttribute);
        card.Attributes.Add(moveAttribute);
        
        // Then move it
        var newPosition = new Vector2I(5, 3); // 2 tiles away
        var moveAction = new MoveUnitAction(_player, card, newPosition);
        Game.Perform(moveAction);
        Game.Update();

        card.BoardPosition.Should().Be(firstPosition);
        Board.GetTile(firstPosition).Objects.Should().Contain(card);
        Board.GetTile(newPosition).Objects.Should().BeEmpty();
    }
    
    [Fact]
    public void BoardSystem_MoveUnitAction_Invalidated_If_NotOnBoard()
    {
        // Draw some cards
        var drawAction = new DrawCardsAction(_player.Id, 5);
        Game.Perform(drawAction);
        Game.Update();
        
        // pick one
        var card = _player.Hand.First();
        
        // set up move attribute
        var moveAttribute = new MoveAttribute(1);
        card.CardData.Attributes.Add(moveAttribute);
        card.Attributes.Add(moveAttribute);
        
        // Then attempt to move it
        var newPosition = new Vector2I(5, 3);
        var moveAction = new MoveUnitAction(_player, card, newPosition);
        Game.Perform(moveAction);
        Game.Update();

        card.BoardPosition.Should().Be(new Vector2I(int.MinValue, int.MinValue));
        Board.GetTile(newPosition).Objects.Should().BeEmpty();
    }
}