using Godot;
using MedievalConquerors.Engine.Actions;
using MedievalConquerors.Engine.Data;
using MedievalConquerors.Engine.Data.Attributes;
using MedievalConquerors.Engine.GameComponents;
using MedievalConquerors.Extensions;
using Shouldly;


namespace MedievalConquerors.Tests.Engine.GameSystemTests;

public class ResourceSystemTests : GameSystemTestFixture
{
    private readonly Player _player;
    private readonly Card _costlyCard;

    public ResourceSystemTests(ITestOutputHelper output, CardLibraryFixture libraryFixture) : base(output, libraryFixture)
    {
        _player = Game.GetComponent<Match>().LocalPlayer;
        _costlyCard = CardBuilder
            .Build(_player)
            .WithCardType(CardType.Unit)
            .WithResourceCost(food: 1)
            .WithSpawnPoint(Tags.TownCenter, 3)
            .Create();

        _player.Deck.Add(_costlyCard);
        // Start the game with the given player
        Game.Awake();
        var action = new BeginGameAction(_player.Id);
        Game.Perform(action);
        Game.Update();
    }

    [Fact]
    public void GameFactory_Creates_ResourceSystem() => Game.GetComponent<ResourceSystem>().ShouldNotBeNull();

    [Fact]
    public void ResourceSystem_Validates_Action_When_Player_Has_Enough_Resources()
    {
        _costlyCard.Zone.ShouldBe(Zone.Hand);
        _player.Hand.ShouldContain(_costlyCard);
        var card = _player.Hand.First();

        var positionToPlay = new Vector2I(5, 5);
        var action = new PlayCardAction(card, positionToPlay);

        var result = action.Validate(Game);

        result.IsValid.ShouldBeTrue();
    }

    [Fact]
    public void ResourceSystem_Invalidates_Action_When_Player_Cannot_Afford_ResourceCost()
    {
        _costlyCard.Zone.ShouldBe(Zone.Hand);
        _player.Hand.ShouldContain(_costlyCard);

        var card = _player.Hand.First();
        card.GetAttribute<ResourceCostAttribute>().Food = 999;

        var positionToPlay = new Vector2I(5, 5);
        var action = new PlayCardAction(card, positionToPlay);

        var result = action.Validate(Game);

        result.IsValid.ShouldBeFalse();
        result.ValidationErrors.ShouldContain(str => str == "Not enough resource to play card.");
    }
}
