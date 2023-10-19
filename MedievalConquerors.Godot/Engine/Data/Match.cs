using MedievalConquerors.Engine.Core;

namespace MedievalConquerors.Engine.Data;

public interface IMatch : IGameComponent
{
    IPlayer LocalPlayer { get; }
    IPlayer EnemyPlayer { get; }
}

public class Match : GameComponent, IMatch
{
    public IPlayer LocalPlayer { get; } = new Player();
    public IPlayer EnemyPlayer { get; } = new Player();
}

