using Godot;
using MedievalConquerors.Engine.Actions;
using MedievalConquerors.Engine.Attributes;
using MedievalConquerors.Engine.Data;
using MedievalConquerors.Engine.GameComponents;
using Shouldly;

namespace MedievalConquerors.Tests.EngineTests.GameSystemTests;

public class MovementSystemTests : GameSystemTestFixture
{
    private readonly Player _player;
    private readonly Match _match;

    private readonly Card _moveableCard;
    private readonly Card _immoveableCard;

    public MovementSystemTests(ITestOutputHelper output, CardLibraryFixture libraryFixture) : base(output, libraryFixture)
    {
        _player = Game.GetComponent<Match>().LocalPlayer;
        _match = Game.GetComponent<Match>();

        _moveableCard = CardBuilder
            .Build(_player)
            .WithCardType(CardType.Unit)
            .WithMovement(1)
            .WithSpawnPoint(Tags.TownCenter, 3)
            .Create();

        _immoveableCard = CardBuilder
            .Build(_player)
            .WithCardType(CardType.Unit)
            .WithSpawnPoint(Tags.TownCenter, 3)
            .Create();

        _player.Deck.Add(_moveableCard);
        _player.Deck.Add(_immoveableCard);

        // Begin the game
        Game.Awake();
        var action = new BeginGameAction(_player.Id);
        Game.Perform(action);
        Game.Update();
    }

    [Fact]
    public void GameFactory_Creates_MovementSystem() => Game.GetComponent<MovementSystem>().ShouldNotBeNull();

    [Fact]
    public void MovementSystem_Performs_MoveUnitAction_And_Moves_Unit()
    {
        _moveableCard.Zone.ShouldBe(Zone.Hand);
        _player.Hand.ShouldContain(_moveableCard);

        // Play the card
        var firstPosition = new Vector2I(5, 5);
        var playAction = new PlayCardAction(_moveableCard, firstPosition);
        Game.Perform(playAction);
        Game.Update();

        // Then move it
        var newPosition = new Vector2I(5, 4);
        var moveAction = new MoveUnitAction(_moveableCard, newPosition);
        Game.Perform(moveAction);
        Game.Update();


        _moveableCard.MapPosition.ShouldBe(newPosition);
        Map.GetTile(newPosition).Unit.ShouldBe(_moveableCard);
        Map.GetTile(firstPosition).Unit.ShouldBeNull();

        _moveableCard.GetAttribute<MovementAttribute>().Distance.ShouldBe(0);
    }

    [Fact]
    public void MovementSystem_MoveUnitAction_Invalidated_Without_MoveAttribute()
    {
        _immoveableCard.Zone.ShouldBe(Zone.Hand);
        _player.Hand.ShouldContain(_immoveableCard);

        // Play the card
        var firstPosition = new Vector2I(5, 5);
        var playAction = new PlayCardAction(_immoveableCard, firstPosition);
        Game.Perform(playAction);
        Game.Update();

        // Then attempt to Move it.
        var newPosition = new Vector2I(5, 4);
        var moveAction = new MoveUnitAction(_immoveableCard, newPosition);
        Game.Perform(moveAction);
        Game.Update();

        _immoveableCard.MapPosition.ShouldBe(firstPosition);
        Map.GetTile(newPosition).Unit.ShouldBeNull();
        Map.GetTile(firstPosition).Unit.ShouldBe(_immoveableCard);
    }

    [Fact]
    public void MovementSystem_MoveUnitAction_Invalidated_If_NotEnoughDistance()
    {
        _moveableCard.Zone.ShouldBe(Zone.Hand);
        _player.Hand.ShouldContain(_moveableCard);

        // Play the card
        var firstPosition = new Vector2I(5, 5);
        var playAction = new PlayCardAction(_moveableCard, firstPosition);
        Game.Perform(playAction);
        Game.Update();

        // Then move it
        var newPosition = new Vector2I(5, 3); // 2 tiles away
        var moveAction = new MoveUnitAction(_moveableCard, newPosition);
        Game.Perform(moveAction);
        Game.Update();

        _moveableCard.MapPosition.ShouldBe(firstPosition);
        Map.GetTile(firstPosition).Unit.ShouldBe(_moveableCard);
        Map.GetTile(newPosition).Unit.ShouldBeNull();
    }

    [Fact]
    public void MovementSystem_MoveUnitAction_Invalidated_If_NotOnMap()
    {
        _moveableCard.Zone.ShouldBe(Zone.Hand);
        _player.Hand.ShouldContain(_moveableCard);

        // attempt to move card in hand
        var newPosition = new Vector2I(5, 3);
        var moveAction = new MoveUnitAction(_moveableCard, newPosition);
        Game.Perform(moveAction);
        Game.Update();

        _moveableCard.MapPosition.ShouldBe(HexMap.None);
        Map.GetTile(newPosition).Unit.ShouldBeNull();
    }

    [Fact]
    public void MovementSystem_OnChangeTurn_ResetsRemainingDistance()
    {
        _moveableCard.Zone.ShouldBe(Zone.Hand);
        _player.Hand.ShouldContain(_moveableCard);

        // Play the card
        var firstPosition = new Vector2I(5, 5);
        var playAction = new PlayCardAction(_moveableCard, firstPosition);
        Game.Perform(playAction);
        Game.Update();

        // Then move it
        var newPosition = new Vector2I(5, 4);
        var moveAction = new MoveUnitAction(_moveableCard, newPosition);
        Game.Perform(moveAction);
        Game.Update();

        var movement = _moveableCard.GetAttribute<MovementAttribute>();
        movement.Distance.ShouldBe(0);

        // Change turn to opposite player
        var turnAction = new ChangeTurnAction(_match.OppositePlayer.Id);
        Game.Perform(turnAction);
        Game.Update();

        // and back to original player
        turnAction = new ChangeTurnAction(_player.Id);
        Game.Perform(turnAction);
        Game.Update();

        movement.Distance.ShouldBe(movement.Distance);
    }
}
