using FluentAssertions;
using Godot;
using MedievalConquerors.Engine.Actions;
using MedievalConquerors.Engine.Core;
using MedievalConquerors.Engine.Data;
using MedievalConquerors.Engine.Data.Attributes;
using MedievalConquerors.Engine.GameComponents;
using MedievalConquerors.Extensions;
using Xunit.Abstractions;

namespace MedievalConquerors.Tests.Engine.GameSystemTests;

public class ResourceSystemTests : GameSystemTestFixture
{
    private readonly Player _player;

    public ResourceSystemTests(ITestOutputHelper output) : base(output)
    {
        _player = Game.GetComponent<Match>().LocalPlayer;
        
        // Start the game with the given player
        var action = new BeginGameAction(_player.Id);
        Game.Perform(action);
        Game.Update();
    }

    [Fact]
    public void ResourceSystem_Validates_Action_When_Player_Has_Enough_Resources()
    {
        var card = _player.Hand.First();

        card.GetAttribute<ResourceCostAttribute>().Food = 1;
        card.GetAttribute<ResourceCostAttribute>().Wood = 0;
        card.GetAttribute<ResourceCostAttribute>().Gold = 0;
        card.GetAttribute<ResourceCostAttribute>().Stone = 0;

        var positionToPlay = new Vector2I(5, 5);
        var action = new PlayCardAction(card, positionToPlay);

        var result = action.Validate(Game);

        result.IsValid.Should().BeTrue();
    }
    
    [Fact]
    public void ResourceSystem_Invalidates_Action_When_Player_Cannot_Afford_ResourceCost()
    {
        var card = _player.Hand.First();

        card.GetAttribute<ResourceCostAttribute>().Food = 999;
        
        var positionToPlay = new Vector2I(5, 5);
        var action = new PlayCardAction(card, positionToPlay);

        var result = action.Validate(Game);

        result.IsValid.Should().BeFalse();
        result.ValidationErrors.Should().Contain(str => str == "Not enough resource to play card.");
    }
}