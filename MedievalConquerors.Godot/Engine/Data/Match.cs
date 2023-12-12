using System.Collections.Generic;
using System.Collections.ObjectModel;
using MedievalConquerors.Engine.Core;

namespace MedievalConquerors.Engine.Data;

public class Match : GameComponent
{
    public const int LocalPlayerId = 0;
    public const int EnemyPlayerId = 1;
    
    public IPlayer LocalPlayer { get; } = new Player(LocalPlayerId);
    public IPlayer EnemyPlayer { get; } = new Player(EnemyPlayerId);

    public IReadOnlyList<IPlayer> Players { get; }
    
    public int CurrentPlayerId { get; set; }

    public IPlayer CurrentPlayer => Players[CurrentPlayerId];
    public IPlayer OppositePlayer => Players[1 - CurrentPlayerId];

    public Match()
    {
        Players = new ReadOnlyCollection<IPlayer>(new[] { LocalPlayer, EnemyPlayer });
    }
}

