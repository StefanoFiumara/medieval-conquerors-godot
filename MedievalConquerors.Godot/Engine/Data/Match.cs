using System.Collections.Generic;
using System.Collections.ObjectModel;
using MedievalConquerors.Engine.Core;

namespace MedievalConquerors.Engine.Data;

public class Match : GameComponent
{
    public const int LOCAL_PLAYER_ID = 0;
    public const int ENEMY_PLAYER_ID = 1;
    
    public IPlayer LocalPlayer { get; } = new Player(LOCAL_PLAYER_ID);
    public IPlayer EnemyPlayer { get; } = new Player(ENEMY_PLAYER_ID);

    public IReadOnlyList<IPlayer> Players { get; }

    public Match()
    {
        Players = new ReadOnlyCollection<IPlayer>(new[] {LocalPlayer, EnemyPlayer});
    }
}

