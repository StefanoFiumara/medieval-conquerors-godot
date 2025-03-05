﻿namespace MedievalConquerors.Engine.Actions.TurnActions;

public class EndTurnAction(int playerId) : GameAction
{
    public int PlayerId { get; private set; } = playerId;

    public override string ToString()
    {
        return $"EndTurnAction: Player {PlayerId} Ends Their Turn";
    }
}
