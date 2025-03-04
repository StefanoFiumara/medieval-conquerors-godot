using System.Collections.Generic;
using Godot;
using MedievalConquerors.Engine.Data;

namespace MedievalConquerors.Engine.Actions;

public class CollectResourcesAction(int targetPlayerId) : GameAction
{
    public int TargetPlayerId { get; } = targetPlayerId;

    public Dictionary<Vector2I, (ResourceType resource, int amount)> ResourcesCollected { get; } = new();
}
