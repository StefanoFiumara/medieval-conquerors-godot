using System;
using System.Collections.Generic;
using Godot;
using MedievalConquerors.Editors;
using MedievalConquerors.Editors.Options;
using MedievalConquerors.Engine.Attributes;
using MedievalConquerors.Engine.Core;
using MedievalConquerors.Engine.Data;

namespace MedievalConquerors.Engine.Actions;

public class CollectResourcesAction(int targetPlayerId, ResourceType resource, int amount) : GameAction, IAbilityLoader
{
    [UseValueEditor(typeof(PlayerTargetOptions))]
    public int TargetPlayerId { get; private set; } = targetPlayerId;
    public ResourceType Resource { get; private set; } = resource;
    public int Amount { get; private set; } = amount;

    public CollectResourcesAction() : this(-1, ResourceType.None, 0) { }

    public static Dictionary<string, Type> GetParameters() =>
        new()
        {
            { nameof(TargetPlayerId), typeof(PlayerTarget) },
            { nameof(Resource), typeof(ResourceType) },
            { nameof(Amount), typeof(int) }
        };

    public void Load(IGame game, Card card, AbilityAttribute ability, ActionDefinition data, Vector2I targetTile)
    {
        TargetPlayerId = this.ResolvePlayerTarget(game, card, data.GetData<PlayerTarget>(nameof(TargetPlayerId)));
        Resource = data.GetData<ResourceType>(nameof(Resource));
        Amount = data.GetData<int>(nameof(Amount));
    }
}
