using Godot;
using MedievalConquerors.Engine.Actions;
using MedievalConquerors.Engine.Attributes;
using MedievalConquerors.Engine.Data;

using MedievalConquerors.Engine.GameComponents;
using MedievalConquerors.Extensions;
using Shouldly;

namespace MedievalConquerors.Tests.Engine.GameSystemTests;

public class AbilitySystemTests : GameSystemTestFixture
{
    private readonly Player _player;
    private readonly Card _cardWithAbility;
    private readonly Player _enemyPlayer;

    public AbilitySystemTests(ITestOutputHelper output, CardLibraryFixture libraryFixture) : base(output,
        libraryFixture)
    {
        _player = Game.GetComponent<Match>().LocalPlayer;
        _enemyPlayer = Game.GetComponent<Match>().EnemyPlayer;

        _cardWithAbility = CardBuilder
            .Build(_player)
            .WithCardType(CardType.Technology)
            .WithAbility<OnCardPlayedAbility, DrawCardsAction>("Amount=2,TargetPlayerId=Owner")
            .WithSpawnPoint(Tags.TownCenter, 3)
            .Create();

        var otherCards = CardBuilder
            .Build(_player)
            .WithCardType(CardType.Unit)
            .WithSpawnPoint(Tags.TownCenter, 3)
            .CreateMany(10);

        var enemyCards = CardBuilder
            .Build(_enemyPlayer)
            .WithCardType(CardType.Unit)
            .WithSpawnPoint(Tags.TownCenter, 3)
            .CreateMany(10);

        _player.Deck.AddRange(otherCards);
        _player.Deck.Add(_cardWithAbility);

        _enemyPlayer.Deck.AddRange(enemyCards);

        // Begin the game
        Game.Awake();
        var action = new BeginGameAction(_player.Id);
        Game.Perform(action);
        Game.Update();
    }

    [Fact]
    public void Ability_Is_Triggered_When_Technology_Card_Is_Played()
    {
        _cardWithAbility.Zone.ShouldBe(Zone.Hand);
        _player.Hand.ShouldContain(_cardWithAbility);

        // The test card triggers an ability that lets the player draw two cards, so let's verify that that action is triggered.
        var originalHandCount = _player.Hand.Except([_cardWithAbility]).Count();

        var action = new PlayCardAction(_cardWithAbility, new Vector2I(5, 3));
        Game.Perform(action);
        Game.Update();

        _cardWithAbility.Zone.ShouldBe(Zone.Banished);
        _player.Banished.ShouldContain(_cardWithAbility);
        _player.Hand.ShouldNotContain(_cardWithAbility);

        _player.Hand.Count.ShouldBe(originalHandCount + 2);
    }

    [Fact]
    public void Ability_Targeting_Specific_Player_Affects_Specified_Player()
    {
        _cardWithAbility.Zone.ShouldBe(Zone.Hand);
        _player.Hand.ShouldContain(_cardWithAbility);

        // Modify ability data to target enemy player
        _cardWithAbility.GetAttribute<OnCardPlayedAbility>().Actions.Single().Data = "Amount=2,TargetPlayerId=Enemy";

        var originalHandCount = _enemyPlayer.Hand.Count;

        var action = new PlayCardAction(_cardWithAbility, new Vector2I(5, 3));
        Game.Perform(action);
        Game.Update();

        _cardWithAbility.Zone.ShouldBe(Zone.Banished);
        _player.Banished.ShouldContain(_cardWithAbility);
        _player.Hand.ShouldNotContain(_cardWithAbility);

        _enemyPlayer.Hand.Count.ShouldBe(originalHandCount + 2);


    }
}
