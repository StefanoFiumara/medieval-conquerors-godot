using System;
using System.Collections.Generic;
using Godot;
using MedievalConquerors.Editors;
using MedievalConquerors.Editors.Options;
using MedievalConquerors.Engine.Attributes;
using MedievalConquerors.Engine.Core;
using MedievalConquerors.Engine.Data;

namespace MedievalConquerors.Engine.Actions;

public class HarvestAction(int targetPlayerId, Vector2I targetTile) : GameAction, IAbilityLoader
{
    [UseValueEditor(typeof(PlayerTargetOptions))]
    public int TargetPlayerId { get; private set; } = targetPlayerId;
    public Vector2I TargetTile { get; private set; } = targetTile;

    public HarvestAction() : this(-1, HexMap.None) { }

    public static Dictionary<string, Type> GetParameters() =>
        new()
        {
            { nameof(TargetPlayerId), typeof(PlayerTarget) },
        };

    public void Load(IGame game, Card card, AbilityAttribute ability, ActionDefinition data, Vector2I targetTile)
    {
        TargetTile = targetTile;
        TargetPlayerId = this.ResolvePlayerTarget(game, card, data.GetData<PlayerTarget>(nameof(TargetPlayerId)));
    }
}
