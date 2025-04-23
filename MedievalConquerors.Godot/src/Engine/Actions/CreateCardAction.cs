using System.Collections.Generic;
using MedievalConquerors.Engine.Attributes;
using MedievalConquerors.Engine.Core;
using MedievalConquerors.Engine.Data;

namespace MedievalConquerors.Engine.Actions;

public class CreateCardAction(int cardId, int targetPlayerId, Zone targetZone, int amount = 1) : GameAction, IAbilityLoader
{
    public int CardId { get; private set; } = cardId;
    public int TargetPlayerId { get; private set; } = targetPlayerId;
    public Zone TargetZone { get; private set; } = targetZone;
    public int Amount { get; private set; } = amount;

    public List<Card> CreatedCards { get; set; } = new();

    public CreateCardAction() : this(-1, -1, Zone.None) { }

    public override string ToString()
    {
        return $"CreateCardAction: Player {TargetPlayerId} creates card {CardId} in {TargetZone}";
    }

    public void Load(IGame game, AbilityAttribute ability, ActionDefinition data)
    {
        var player = ability.Owner.Owner;
        var enemyPlayer = game.GetComponent<Match>().Players[1 - player.Id];
        var target = data.GetData<PlayerTarget>(nameof(TargetPlayerId));

        TargetPlayerId = target == PlayerTarget.Owner ? player.Id : enemyPlayer.Id;
        CardId = data.GetData<int>(nameof(CardId));
        TargetZone = data.GetData<Zone>(nameof(TargetZone));
        Amount = data.GetData<int>(nameof(Amount));
    }
}
