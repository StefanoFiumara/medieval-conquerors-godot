using Godot;
using MedievalConquerors.Engine.Data;

namespace MedievalConquerors.Engine.Actions;

public class BuildStructureAction(Card structureToBuild, Vector2I targetTile) : GameAction
{
    public Card StructureToBuild { get; } = structureToBuild;
    public Vector2I TargetTile { get; } = targetTile;

    public override string ToString()
    {
        return $"BuildStructureAction: Player {StructureToBuild.Owner.Id} Builds {StructureToBuild.Data.Title} at {TargetTile}";
    }
}
