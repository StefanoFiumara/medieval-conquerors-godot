using System.Collections.Generic;
using Godot;
using MedievalConquerors.Engine.Data;

namespace MedievalConquerors.Engine.Actions;

public class CollectResourcesAction : GameAction
{
    public int TargetPlayerId { get; }

    public Dictionary<Vector2I, (ResourceType resource, int amount)> ResourcesCollected { get; } = new();

    public CollectResourcesAction(int targetPlayerId)
    {
        TargetPlayerId = targetPlayerId;
    }
}