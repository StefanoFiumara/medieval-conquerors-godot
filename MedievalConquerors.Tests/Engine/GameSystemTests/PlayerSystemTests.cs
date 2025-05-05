using Godot;
using MedievalConquerors.Engine.Actions;
using MedievalConquerors.Engine.Data;
using MedievalConquerors.Engine.GameComponents;
using Shouldly;


namespace MedievalConquerors.Tests.Engine.GameSystemTests;

public class PlayerSystemTests : GameSystemTestFixture
{
    private readonly Player _player;

    public PlayerSystemTests(ITestOutputHelper output, CardLibraryFixture libraryFixture) : base(output, libraryFixture)
    {
        _player = Game.GetComponent<Match>().LocalPlayer;

        var cards = CardBuilder
            .Build(_player)
            .WithCardType(CardType.Unit)
            .WithMovement(1)
            .WithSpawnPoint(Tags.TownCenter, 3)
            .CreateMany(2);

        _player.Deck.AddRange(cards);

        // Start the game with the given player
        Game.Awake();
        var action = new BeginGameAction(_player.Id);
        Game.Perform(action);
        Game.Update();
    }

    [Fact]
    public void GameFactory_Creates_PlayerSystem() => Game.GetComponent<PlayerSystem>().ShouldNotBeNull();

    // TODO: On Begin game, player system assigns town centers and starting resources

    [Fact]
    public void PlayerSystem_Performs_PlayCardAction_And_Moves_To_MapZone()
    {
        // Play a card
        var cardToPlay = _player.Hand.Last();

        var positionToPlay = new Vector2I(5, 5);
        var playAction = new PlayCardAction(cardToPlay, positionToPlay);

        Game.Perform(playAction);
        Game.Update();

        _player.Map.Count.ShouldBe(2);
        _player.Hand.Count.ShouldBe(2);

        cardToPlay.Zone.ShouldBe(Zone.Map);
        cardToPlay.MapPosition.ShouldBe(positionToPlay);

        var tile = Map.GetTile(positionToPlay);

        tile.Unit.ShouldNotBeNull();
        tile.Unit.ShouldBe(cardToPlay);
    }

    [Fact]
    public void PlayerSystem_Performs_DiscardCardsAction_And_Moves_To_DiscardZone()
    {
        // Play a card
        var card = _player.Hand.Last();
        var positionToPlay = new Vector2I(5, 5);
        var playAction = new PlayCardAction(card, positionToPlay);
        Game.Perform(playAction);
        Game.Update();

        var discardAction = new DiscardCardsAction([card]);

        Game.Perform(discardAction);
        Game.Update();

        _player.Map.ShouldHaveSingleItem(); // the town center
        _player.Discard.Count.ShouldBe(1);
        _player.Discard.ShouldAllBe(c => c.Zone == Zone.Discard);

        Map.GetTile(positionToPlay).Unit.ShouldBeNull();
        card.MapPosition.ShouldBe(HexMap.None);
    }

    [Fact]
    public void PlayerSystem_Performs_DiscardCardsAction_And_Moves_Multiple_Cards_To_DiscardZone()
    {
        // Discard the player's hand
        var toDiscard = _player.Hand.ToList();
        var discardAction = new DiscardCardsAction(toDiscard);

        Game.Perform(discardAction);
        Game.Update();

        _player.Hand.Count.ShouldBe(0);
        _player.Discard.Count.ShouldBe(2);
        _player.Discard.ShouldAllBe(c => c.Zone == Zone.Discard);
    }
}
