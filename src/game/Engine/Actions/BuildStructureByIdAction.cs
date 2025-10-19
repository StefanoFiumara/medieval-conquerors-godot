using Godot;
using MedievalConquerors.Engine.Attributes;
using MedievalConquerors.Engine.Core;
using MedievalConquerors.Engine.Data;

namespace MedievalConquerors.Engine.Actions;

public class BuildStructureByIdAction(int cardId, int ownerId, Vector2I targetTile) : GameAction, IAbilityLoader
{
    public int CardId { get; private set; } = cardId;
    public int OwnerId { get; set; } = ownerId;
    public Vector2I TargetTile { get; set; } = targetTile;

    public BuildStructureByIdAction() : this(-1, -1, HexMap.None) { }

    public void Load(IGame game, Card card, AbilityAttribute ability, ActionDefinition data, Vector2I targetTile)
    {
        var player = card.Owner;
        OwnerId = player.Id;
        TargetTile = targetTile;
        CardId = data.GetData<int>(nameof(CardId));
    }
}
