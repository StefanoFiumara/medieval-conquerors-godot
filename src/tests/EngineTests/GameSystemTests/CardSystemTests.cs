using MedievalConquerors.Engine.Actions;
using MedievalConquerors.Engine.Data;
using MedievalConquerors.Engine.GameComponents;
using Shouldly;

namespace MedievalConquerors.Tests.EngineTests.GameSystemTests;

public class CardSystemTests : GameSystemTestFixture
{
    private readonly Player _player;
    private readonly CardSystem _cardSystem;

    public CardSystemTests(ITestOutputHelper output, CardLibraryFixture libraryFixture) : base(output, libraryFixture)
    {
        _player = Game.GetComponent<Match>().LocalPlayer;
        _cardSystem = Game.GetComponent<CardSystem>();
    }

    [Fact]
    public void GameFactory_Creates_CardSystem()
    {
        Game.GetComponent<CardSystem>().ShouldNotBeNull();
    }

    [Fact]
    public void CardSystem_Marks_Cards_As_Playable()
    {
        var card = CardBuilder.Build(_player)
            .WithSpawnPoint(Tags.TownCenter, 3)
            .Create();
        _player.Deck.Add(card);

        Game.Awake();
        Game.Perform(new BeginGameAction(_player.Id));
        Game.Update();

        _cardSystem.IsPlayable(card).ShouldBeTrue();
    }

    [Fact]
    public void CardSystem_Marks_Card_Unplayable_When_PlayCondition_Is_Unmet()
    {
        var card = CardBuilder.Build(_player)
            .WithSpawnPoint(Tags.TownCenter, 3)
            .WithResourceCost(food: 99)
            .Create();
        _player.Deck.Add(card);

        Game.Awake();
        Game.Perform(new BeginGameAction(_player.Id));
        Game.Update();

        _cardSystem.IsPlayable(card).ShouldBeFalse();
    }
}
