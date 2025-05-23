using MedievalConquerors.Engine.Core;

namespace MedievalConquerors.Tests.Engine;

public class GameComponentTests
{
    private readonly Game _game = new();

    [Fact]
    public void GameEntity_Can_Add_Components()
    {
        var component = new GameComponent();

        _game.AddComponent(component);

        Assert.Single(_game.Components);
    }

    [Fact]
    public void GameEntity_Added_Component_Can_Be_Fetched_By_Type()
    {
        var component = new GameComponent();

        _game.AddComponent(component);

        var actual = _game.GetComponent<GameComponent>();

        Assert.Equal(component, actual);
    }

    [Fact]
    public void GameEntity_AddComponent_Creates_New_Component()
    {
        _game.AddComponent<GameComponent>();

        var component = _game.GetComponent<GameComponent>();

        Assert.NotNull(component);
        Assert.IsType<GameComponent>(component, exactMatch: false);
    }

    [Fact]
    public void GameEntity_AddComponent_References_Parent()
    {
        _game.AddComponent<GameComponent>();

        var component = _game.GetComponent<GameComponent>();

        Assert.NotNull(component);
        Assert.NotNull(component.Game);
        Assert.Equal(_game, component.Game);
    }
}
