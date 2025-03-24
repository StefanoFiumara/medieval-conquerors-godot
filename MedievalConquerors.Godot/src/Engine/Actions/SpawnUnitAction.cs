using Godot;
using MedievalConquerors.Engine.Data;

namespace MedievalConquerors.Engine.Actions;

public class SpawnUnitAction(Card unitToSpawn, Vector2I targetTile) : GameAction
{
    public Card UnitToSpawn { get; } = unitToSpawn;
    public Vector2I TargetTile { get; } = targetTile;

    public override string ToString()
    {
        return $"SpawnUnitAction: Player {UnitToSpawn.Owner.Id} spawns {UnitToSpawn.CardData.Title} at {TargetTile}";
    }
}
