using System.Collections.Generic;
using System.Collections.ObjectModel;
using MedievalConquerors.Engine.Core;

namespace MedievalConquerors.Engine.Data;

public class Match : GameComponent
{
    public const int LOCAL_PLAYER_ID = 0;
    public const int ENEMY_PLAYER_ID = 1;
    
    public Player LocalPlayer { get; } = new Player(LOCAL_PLAYER_ID);
    public Player EnemyPlayer { get; } = new Player(ENEMY_PLAYER_ID);

    public IReadOnlyList<Player> Players { get; }
    
    public int CurrentPlayerId { get; set; }

    public Player CurrentPlayer => Players[CurrentPlayerId];
    public Player OppositePlayer => Players[1 - CurrentPlayerId];

    public Match()
    {
        Players = new ReadOnlyCollection<Player>([LocalPlayer, EnemyPlayer]);
    }
}

