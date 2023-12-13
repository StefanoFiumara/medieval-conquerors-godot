﻿using FluentAssertions;
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
    }

    [Fact]
    public void GameFactory_Creates_PlayerSystem()
    {
        Game.GetComponent<PlayerSystem>().Should().NotBeNull();
    }
    
    [Fact]
    public void PlayerSystem_Performs_DiscardCardsAction_And_Discards_From_Hand()
    {
        // Draw 5 cards
        var drawAction = new DrawCardsAction(_player.Id, 5);
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
    public void PlayerSystem_Performs_PlayCardAction_And_PlacesOnBoard()
    {
        // Draw some cards
        var drawAction = new DrawCardsAction(_player.Id, 5);
        Game.Perform(drawAction);
        Game.Update();
        
        // Then play one
        var cardToPlay = _player.Hand.First();
        var positionToPlay = new Vector2I(5, 5);
        var playAction = new PlayCardAction(_player, cardToPlay, positionToPlay);
        
        Game.Perform(playAction);
        Game.Update();

        _player.Board.Should().HaveCount(1);
        _player.Hand.Should().HaveCount(4);
        _player.Board.Should().HaveCount(1);
        
        cardToPlay.Zone.Should().Be(Zone.Board);
        cardToPlay.BoardPosition.Should().Be(positionToPlay);
        
        var tile = Board.GetTile(positionToPlay);
        
        tile.Objects.Should().HaveCount(1);
        tile.Objects.Single().Should().Be(cardToPlay);
    }
    
    [Fact]
    public void PlayerSystem_Performs_DiscardCardsAction_And_Discards_From_Board()
    {
        // Draw some cards
        var drawAction = new DrawCardsAction(_player.Id, 5);
        Game.Perform(drawAction);
        Game.Update();
        
        // Then play one
        var cardToPlay = _player.Hand.First();
        var positionToPlay = new Vector2I(5, 5);
        var playAction = new PlayCardAction(_player, cardToPlay, positionToPlay);
        Game.Perform(playAction);
        Game.Update();
        
        // Then discard it
        var toDiscard = _player.Board.Take(1).ToList();
        var discardAction = new DiscardCardsAction(toDiscard, _player);

        Game.Perform(discardAction);
        Game.Update();
        
        _player.Board.Should().BeEmpty();
        _player.Discard.Should().HaveCount(1);
        _player.Discard.Should().AllSatisfy(c => c.Zone.Should().Be(Zone.Discard));

        Board.GetTile(positionToPlay).Objects.Should().BeEmpty();
    }
}