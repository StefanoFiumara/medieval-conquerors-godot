namespace MedievalConquerors.Engine.Core;

public interface IGameComponent
{
    IGame Game { get; set; }
}

public class GameComponent : IGameComponent
{
    public IGame Game {get; set; }
}