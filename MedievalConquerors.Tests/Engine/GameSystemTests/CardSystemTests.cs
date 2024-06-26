using FluentAssertions;
using MedievalConquerors.Engine.Actions;
using MedievalConquerors.Engine.Core;
using MedievalConquerors.Engine.Data;
using MedievalConquerors.Engine.Data.Attributes;
using MedievalConquerors.Engine.GameComponents;
using Xunit.Abstractions;

namespace MedievalConquerors.Tests.Engine.GameSystemTests;

public class CardSystemTests : GameSystemTestFixture
{
    private readonly Player _player;
    private CardSystem _underTest;

    public CardSystemTests(ITestOutputHelper output) : base(output)
    {
        _player = Game.GetComponent<Match>().LocalPlayer;
        _underTest = Game.GetComponent<CardSystem>();
        
        // Start the game with the given player
        var action = new BeginGameAction(_player.Id);
        Game.Perform(action);
        Game.Update();
    }

    [Fact]
    public void GameFactory_Creates_CardSystem()
    {
        Game.GetComponent<CardSystem>().Should().NotBeNull();
    }

    [Fact]
    public void CardSystem_Marks_Cards_As_Playable()
    {
        var card = _player.Hand.First();
        // clear existing cost attributes
        card.Attributes.Clear();
        
        _underTest.Refresh();
        
        _underTest = Game.GetComponent<CardSystem>();
        _underTest.IsPlayable(card)
            .Should().BeTrue();
    }

    [Fact]
    public void CardSystem_Marks_Card_Unplayable_When_PlayCondition_Is_Unmet()
    {
        var card = _player.Hand.First();
        
        card.Attributes.Clear();
        card.Attributes.Add(new ResourceCostAttribute
        {
            Food = 999
        });
        
        _underTest.Refresh();

        _underTest.IsPlayable(card).Should().BeFalse();
    }
}