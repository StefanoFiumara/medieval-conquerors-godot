using Godot;
using MedievalConquerors.Engine.Actions;
using MedievalConquerors.Engine.Data;
using MedievalConquerors.Engine.GameComponents;
using Shouldly;

namespace MedievalConquerors.Tests.EngineTests.GameSystemTests;

public class MapSystemTests : GameSystemTestFixture
{
    private readonly Player _player;
    private readonly Match _match;

    private readonly Card _moveableCard;

    public MapSystemTests(ITestOutputHelper output, CardLibraryFixture libraryFixture) : base(output, libraryFixture)
    {
        _player = Game.GetComponent<Match>().LocalPlayer;
        _match = Game.GetComponent<Match>();

        _moveableCard = CardBuilder
            .Build(_player)
            .WithCardType(CardType.Unit)
            .WithMovement(1)
            .WithSpawnPoint(Tags.TownCenter, 3)
            .Create();

        _player.Deck.Add(_moveableCard);

        // Begin the game
        Game.Awake();
        var action = new BeginGameAction(_player.Id);
        Game.Perform(action);
        Game.Update();
    }

    [Fact]
    public void GameFactory_Creates_MapSystem() => Game.GetComponent<MapSystem>().ShouldNotBeNull();



    [Fact]
    public void MapSystem_Performs_DiscardCardsAction_And_Removes_From_Map()
    {
        _moveableCard.Zone.ShouldBe(Zone.Hand);
        _player.Hand.ShouldContain(_moveableCard);

        // Play the card
        var positionToPlay = new Vector2I(5, 5);
        var playAction = new PlayCardAction(_moveableCard, positionToPlay);
        Game.Perform(playAction);
        Game.Update();

        // Then discard it
        var discardAction = new DiscardCardsAction([_moveableCard]);

        Game.Perform(discardAction);
        Game.Update();

        Map.GetTile(positionToPlay).Unit.ShouldBeNull();
        _moveableCard.MapPosition.ShouldBe(HexMap.None);
    }
}
