using MedievalConquerors.Engine.Core;
using MedievalConquerors.Engine.Data;

namespace MedievalConquerors.Engine.GameComponents;

public interface IMatch : IGameComponent
{
    IPlayer LocalPlayer { get; }
    IPlayer EnemyPlayer { get; }
}

