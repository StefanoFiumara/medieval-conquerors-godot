using FluentAssertions;
using MedievalConquerors.Engine.GameComponents;
using Xunit.Abstractions;

namespace MedievalConquerors.Tests.Engine.GameSystemTests;

public class BoardSystemTests : GameSystemTestFixture
{
    public BoardSystemTests(ITestOutputHelper output) : base(output)
    {
    }

    [Fact]
    public void GameFactory_Creates_BoardSystem()
    {
        Game.GetComponent<BoardSystem>().Should().NotBeNull();
    }
}