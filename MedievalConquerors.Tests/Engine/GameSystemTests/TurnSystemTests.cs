using FluentAssertions;
using MedievalConquerors.Engine.Actions;
using MedievalConquerors.Engine.Core;
using MedievalConquerors.Engine.Data;
using MedievalConquerors.Engine.GameComponents;
using Xunit.Abstractions;

namespace MedievalConquerors.Tests.Engine.GameSystemTests;

public class TurnSystemTests : GameSystemTestFixture
{
    private readonly TurnSystem _underTest;

    public TurnSystemTests(ITestOutputHelper output) : base(output)
    {
        _underTest = Game.GetComponent<TurnSystem>();
    }

    [Fact]
    public void GameFactory_Creates_TurnSystem()
    {
        Assert.NotNull(_underTest);
    }

    [Theory]
    [InlineData(Match.LocalPlayerId)]
    [InlineData(Match.EnemyPlayerId)]
    public void Match_Tracks_CurrentPlayer_Id(int nextPlayerId)
    {
        var action = new ChangeTurnAction(nextPlayerId);
        
        Game.Perform(action);
        Game.Update();

        var match = Game.GetComponent<Match>();
        match.CurrentPlayerId.Should().Be(nextPlayerId);
        match.CurrentPlayer.Id.Should().Be(nextPlayerId);
    }

    [Theory]
    [InlineData(Match.LocalPlayerId)]
    [InlineData(Match.EnemyPlayerId)]
    public void BeginGameAction_Begins_Game_With_Player_And_Draws_Initial_Hand(int startingPlayerId)
    {
        var action = new BeginGameAction(startingPlayerId);
        
        Game.Perform(action);
        Game.Update();
        
        var match = Game.GetComponent<Match>();
        match.CurrentPlayerId.Should().Be(startingPlayerId);
        match.CurrentPlayer.Id.Should().Be(startingPlayerId);

        match.CurrentPlayer.Hand.Should().HaveCount(6); // 5 starting cards, + turn draw.
        match.OppositePlayer.Hand.Should().HaveCount(5); // 5 starting cards
    }

}