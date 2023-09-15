using MedievalConquerors.Engine.Core;
using MedievalConquerors.Engine.Data;

namespace MedievalConquerors.Engine.GameComponents;

public interface IMatch : IGameComponent
{
    IGameBoard GameBoard { get; }
    IPlayer LocalPlayer { get; }
    IPlayer EnemyPlayer { get; }
}

