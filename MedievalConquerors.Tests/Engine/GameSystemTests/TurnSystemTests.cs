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
}