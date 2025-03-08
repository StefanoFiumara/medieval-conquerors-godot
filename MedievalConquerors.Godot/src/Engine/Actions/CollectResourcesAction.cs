using System.Collections.Generic;
using Godot;
using MedievalConquerors.Engine.Data;

namespace MedievalConquerors.Engine.Actions;

public class CollectResourcesAction(int targetPlayerId) : GameAction
{
    public int TargetPlayerId { get; } = targetPlayerId;

    public List<(Vector2I position, ResourceType resource, int amount)> ResourcesCollected { get; } = new();
}
