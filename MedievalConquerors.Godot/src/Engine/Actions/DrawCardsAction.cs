using System.Collections.Generic;
using MedievalConquerors.Engine.Core;
using MedievalConquerors.Engine.Data;
using MedievalConquerors.Engine.Data.Attributes;
using MedievalConquerors.Engine.GameComponents;

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

    public void Load(IGame game, AbilityAttribute ability, ActionDefinition data)
    {
        var player = ability.Owner.Owner;
        var enemyPlayer = game.GetComponent<Match>().Players[1 - player.Id];

        var target = data.GetData<PlayerTarget>(nameof(TargetPlayerId));
        TargetPlayerId = target == PlayerTarget.Owner ? player.Id : enemyPlayer.Id;

        Amount = data.GetData<int>(nameof(Amount));
    }
}
