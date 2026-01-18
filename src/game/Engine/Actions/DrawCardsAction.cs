using System;
using System.Collections.Generic;
using Godot;
using MedievalConquerors.Engine.Attributes;
using MedievalConquerors.Engine.Core;
using MedievalConquerors.Engine.Data;

namespace MedievalConquerors.Engine.Actions;

public class DrawCardsAction(int targetPlayerId, int amount) : GameAction, IAbilityLoader
{
    public int TargetPlayerId { get; private set; } = targetPlayerId;
    public int Amount { get; private set; } = amount;
    public List<Card> DrawnCards { get; } = [];

    public DrawCardsAction() : this(-1, 0) { }

    public override string ToString()
    {
        return $"DrawCardsAction: Player {TargetPlayerId} Draws {Amount} card(s)";
    }

    public static Dictionary<string, Type> GetParameters(Type actionType)
    {
        throw new NotImplementedException();
    }

    public void Load(IGame game, Card card, AbilityAttribute ability, ActionDefinition data, Vector2I targetTile)
    {
        var player = card.Owner;
        var enemyPlayer = game.GetComponent<Match>().Players[1 - player.Id];

        var target = data.GetData<PlayerTarget>(nameof(TargetPlayerId));
        TargetPlayerId = target == PlayerTarget.Owner ? player.Id : enemyPlayer.Id;

        Amount = data.GetData<int>(nameof(Amount));
    }
}
