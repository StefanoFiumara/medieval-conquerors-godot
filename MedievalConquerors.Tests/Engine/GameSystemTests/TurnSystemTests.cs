using MedievalConquerors.Engine.Actions;
using MedievalConquerors.Engine.Actions.TurnActions;
using MedievalConquerors.Engine.GameComponents;
using Shouldly;


namespace MedievalConquerors.Tests.Engine.GameSystemTests;

public class TurnSystemTests : GameSystemTestFixture
{
    private readonly TurnSystem _underTest;

    public TurnSystemTests(ITestOutputHelper output, CardLibraryFixture libraryFixture) : base(output, libraryFixture)
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
        // Start the game with the given player
        var player = Game.GetComponent<Match>().LocalPlayer;
        var beginGameAction = new BeginGameAction(player.Id);
        Game.Perform(beginGameAction);
        Game.Update();
        
        var turnAction = new ChangeTurnAction(nextPlayerId);
        
        Game.Perform(turnAction);
        Game.Update();

        var match = Game.GetComponent<Match>();
        match.CurrentPlayerId.ShouldBe(nextPlayerId);
        match.CurrentPlayer.Id.ShouldBe(nextPlayerId);
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
        match.CurrentPlayerId.ShouldBe(startingPlayerId);
        match.CurrentPlayer.Id.ShouldBe(startingPlayerId);

        match.CurrentPlayer.Hand.Count.ShouldBe(6); // 5 starting cards, + turn draw.
        match.OppositePlayer.Hand.Count.ShouldBe(5); // 5 starting cards
    }
}