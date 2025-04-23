using MedievalConquerors.Engine.Actions;
using MedievalConquerors.Engine.Data;
using MedievalConquerors.Engine.GameComponents;
using Shouldly;


namespace MedievalConquerors.Tests.Engine.GameSystemTests;

public class TurnSystemTests : GameSystemTestFixture
{
    private readonly TurnSystem _turnSystem;

    public TurnSystemTests(ITestOutputHelper output, CardLibraryFixture libraryFixture) : base(output, libraryFixture)
    {
        _turnSystem = Game.GetComponent<TurnSystem>();
        Game.Awake();
    }

    [Fact]
    public void GameFactory_Creates_TurnSystem() => _turnSystem.ShouldNotBeNull();

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

        var match = Game.GetComponent<Match>();

        match.CurrentPlayer.Id.ShouldBe(player.Id);
        match.CurrentPlayerId.ShouldBe(player.Id);

        var turnAction = new ChangeTurnAction(nextPlayerId);
        Game.Perform(turnAction);
        Game.Update();

        match.CurrentPlayerId.ShouldBe(nextPlayerId);
        match.CurrentPlayer.Id.ShouldBe(nextPlayerId);
    }
}
