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

        // Start the game with the given player
        var action = new BeginGameAction(_player.Id);
        Game.Perform(action);
        Game.Update();
    }

    [Fact]
    public void GameFactory_Creates_PlayerSystem()
    {
        Game.GetComponent<PlayerSystem>().ShouldNotBeNull();
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

        _player.Map.Count.ShouldBe(1);
        _player.Hand.Count.ShouldBe(5);
        _player.Map.Count.ShouldBe(1);

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
        var cardToPlay = _player.Hand.First();
        var positionToPlay = new Vector2I(5, 5);
        var playAction = new PlayCardAction(cardToPlay, positionToPlay);
        Game.Perform(playAction);
        Game.Update();

        // Then discard it
        var toDiscard = _player.Map.Take(1).ToList();
        var discardAction = new DiscardCardsAction(toDiscard);

        Game.Perform(discardAction);
        Game.Update();

        _player.Map.ShouldBeEmpty();
        _player.Discard.Count.ShouldBe(1);
        _player.Discard.ShouldAllBe(c => c.Zone == Zone.Discard);

        Map.GetTile(positionToPlay).Unit.ShouldBeNull();
        toDiscard.Single().MapPosition.ShouldBe(HexMap.None);
    }

    [Fact]
    public void PlayerSystem_Performs_DiscardCardsAction_And_Moves_Multiple_Cards_To_DiscardZone()
    {
        // Discard 2 random cards
        var toDiscard = _player.Hand.Take(2).ToList();
        var discardAction = new DiscardCardsAction(toDiscard);

        Game.Perform(discardAction);
        Game.Update();

        _player.Hand.Count.ShouldBe(4);
        _player.Discard.Count.ShouldBe(2);
        _player.Discard.ShouldAllBe(c => c.Zone == Zone.Discard);
    }
}
