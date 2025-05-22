using Godot;
using MedievalConquerors.Engine.Attributes;
using MedievalConquerors.Engine.Core;
using MedievalConquerors.Engine.Data;

namespace MedievalConquerors.Engine.Actions;

public class ShuffleDeckAction(int targetPlayerId) : GameAction, IAbilityLoader
{
    public int TargetPlayerId { get; private set; } = targetPlayerId;

    public ShuffleDeckAction() : this(-1) { }

    public override string ToString()
    {
        return $"ShuffleDeckAction:\tPlayer {TargetPlayerId} Shuffles their deck";
    }

    public void Load(IGame game, AbilityAttribute ability, ActionDefinition data, Vector2I targetTile)
    {
        var player = ability.Owner.Owner;
        var enemyPlayer = game.GetComponent<Match>().Players[1 - player.Id];

        var target = data.GetData<PlayerTarget>(nameof(TargetPlayerId));
        TargetPlayerId = target == PlayerTarget.Owner ? player.Id : enemyPlayer.Id;
    }
}
