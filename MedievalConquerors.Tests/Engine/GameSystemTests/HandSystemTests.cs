using MedievalConquerors.Engine.GameComponents;
using Xunit.Abstractions;

namespace MedievalConquerors.Tests.Engine.GameSystemTests;

public class HandSystemTests : GameSystemTestFixture
{
    private readonly HandSystem _underTest;
    
    public HandSystemTests(ITestOutputHelper output) : base(output)
    {
        _underTest = Game.GetComponent<HandSystem>();
    }

    [Fact]
    public void GameFactory_Creates_HandSystem()
    {
        Assert.NotNull(_underTest);
    }
}