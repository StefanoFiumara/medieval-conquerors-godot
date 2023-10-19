using MedievalConquerors.Engine.GameComponents;
using Xunit.Abstractions;

namespace MedievalConquerors.Tests.Engine;

public class HandSystemTests : GameSystemTestFixture
{
    private readonly HandSystem _underTest;
    
    public HandSystemTests(ITestOutputHelper output) : base(output)
    {
        _underTest = Game.GetComponent<HandSystem>();
    }
}