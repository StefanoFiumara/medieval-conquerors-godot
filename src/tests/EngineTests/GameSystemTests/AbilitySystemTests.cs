using Godot;
using MedievalConquerors.Engine.Actions;
using MedievalConquerors.Engine.Attributes;
using MedievalConquerors.Engine.Data;
using MedievalConquerors.Engine.GameComponents;
using MedievalConquerors.Extensions;
using Shouldly;

namespace MedievalConquerors.Tests.EngineTests.GameSystemTests;

public class AbilitySystemTests : GameSystemTestFixture
{
    private readonly Player _player;
    private readonly Card _cardTargetingSelf;
    private readonly Card _cardTargetingEnemy;
    private readonly Player _enemyPlayer;

    public AbilitySystemTests(ITestOutputHelper output, CardLibraryFixture libraryFixture) : base(output,
        libraryFixture)
    {
        _player = Game.GetComponent<Match>().LocalPlayer;
        _enemyPlayer = Game.GetComponent<Match>().EnemyPlayer;

        _cardTargetingSelf = CardBuilder
            .Build(_player)
            .WithCardType(CardType.Technology)
            .WithAbility<OnCardPlayedAbility, DrawCardsAction>("Amount=2,TargetPlayerId=Owner")
            .WithSpawnPoint(Tags.TownCenter, 3)
            .Create();

        _cardTargetingEnemy = CardBuilder
            .Build(_player)
            .WithCardType(CardType.Technology)
            .WithAbility<OnCardPlayedAbility, DrawCardsAction>("Amount=2,TargetPlayerId=Enemy")
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
        _player.Deck.Add(_cardTargetingSelf);
        _player.Deck.Add(_cardTargetingEnemy);

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
        _cardTargetingSelf.Zone.ShouldBe(Zone.Hand);
        _player.Hand.ShouldContain(_cardTargetingSelf);

        // The test card triggers an ability that lets the player draw two cards, so let's verify that that action is triggered.
        var originalHandCount = _player.Hand.Except([_cardTargetingSelf]).Count();

        var action = new PlayCardAction(_cardTargetingSelf, new Vector2I(5, 3));
        Game.Perform(action);
        Game.Update();

        _cardTargetingSelf.Zone.ShouldBe(Zone.Banished);
        _player.Banished.ShouldContain(_cardTargetingSelf);
        _player.Hand.ShouldNotContain(_cardTargetingSelf);

        _player.Hand.Count.ShouldBe(originalHandCount + 2);
    }

    [Fact]
    public void Ability_Targeting_Specific_Player_Affects_Specified_Player()
    {
        _cardTargetingEnemy.Zone.ShouldBe(Zone.Hand);
        _player.Hand.ShouldContain(_cardTargetingEnemy);

        var originalHandCount = _enemyPlayer.Hand.Count;

        var action = new PlayCardAction(_cardTargetingEnemy, new Vector2I(5, 3));
        Game.Perform(action);
        Game.Update();

        _cardTargetingEnemy.Zone.ShouldBe(Zone.Banished);
        _player.Banished.ShouldContain(_cardTargetingEnemy);
        _player.Hand.ShouldNotContain(_cardTargetingEnemy);

        _enemyPlayer.Hand.Count.ShouldBe(originalHandCount + 2);
    }
}
