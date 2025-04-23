using System.Collections.Generic;
using System.Collections.ObjectModel;
using MedievalConquerors.Engine.Core;

namespace MedievalConquerors.Engine.Data;

public class Match : GameComponent
{
    public const int LocalPlayerId = 0;
    public const int EnemyPlayerId = 1;
    
    public Player LocalPlayer { get; } = new Player(LocalPlayerId);
    public Player EnemyPlayer { get; } = new Player(EnemyPlayerId);

    public IReadOnlyList<Player> Players { get; }
    
    public int CurrentPlayerId { get; set; }

    public Player CurrentPlayer => Players[CurrentPlayerId];
    public Player OppositePlayer => Players[1 - CurrentPlayerId];

    public Match()
    {
        Players = new ReadOnlyCollection<Player>([LocalPlayer, EnemyPlayer]);
    }
}

