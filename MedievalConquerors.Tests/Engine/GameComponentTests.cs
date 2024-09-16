using MedievalConquerors.Engine.Core;

namespace MedievalConquerors.Tests.Engine;

public class GameComponentTests
{
    private readonly Game _underTest = new();

    [Fact]
    public void GameEntity_Can_Add_Components()
    {
        var component = new GameComponent();

        _underTest.AddComponent(component);
        
        Assert.Equal(1, _underTest.Components.Count);
    }

    [Fact]
    public void GameEntity_Added_Component_Can_Be_Fetched_By_Type()
    {
        var component = new GameComponent();

        _underTest.AddComponent(component);

        var actual = _underTest.GetComponent<GameComponent>();
        
        Assert.Equal(component, actual);
    }
    
    [Fact]
    public void GameEntity_AddComponent_Creates_New_Component()
    {
        _underTest.AddComponent<GameComponent>();

        var component = _underTest.GetComponent<GameComponent>();
        
        Assert.NotNull(component);
        Assert.IsAssignableFrom<GameComponent>(component);
    }

    [Fact]
    public void GameEntity_AddComponent_References_Parent()
    {
        _underTest.AddComponent<GameComponent>();

        var component = _underTest.GetComponent<GameComponent>();
        
        Assert.NotNull(component);
        Assert.NotNull(component.Game);
        Assert.Equal(_underTest, component.Game);
    }
}