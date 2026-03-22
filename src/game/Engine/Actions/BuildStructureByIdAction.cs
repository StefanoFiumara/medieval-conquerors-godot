using System;
using System.Collections.Generic;
using Godot;
using MedievalConquerors.Editors;
using MedievalConquerors.Editors.CustomEditors.ValueEditors;
using MedievalConquerors.Editors.Options;
using MedievalConquerors.Engine.Attributes;
using MedievalConquerors.Engine.Core;
using MedievalConquerors.Engine.Data;

namespace MedievalConquerors.Engine.Actions;

public class BuildStructureByIdAction(int cardId, int ownerId, Vector2I targetTile) : GameAction, IAbilityLoader
{
    [UseValueEditor(typeof(CardIdSelector))]
    public int CardId { get; private set; } = cardId;

    [UseValueEditor(typeof(PlayerTargetOptions))]
    public int OwnerId { get; set; } = ownerId;

    public Vector2I TargetTile { get; set; } = targetTile;

    public BuildStructureByIdAction() : this(-1, -1, HexMap.None) { }

    public static Dictionary<string, Type> GetParameters() =>
        new()
        {
            { nameof(OwnerId), typeof(PlayerTarget) },
            { nameof(CardId), typeof(int) }
        };

    public void Load(IGame game, Card card, AbilityAttribute ability, ActionDefinition data, Vector2I targetTile)
    {
        TargetTile = targetTile;
        OwnerId = this.ResolvePlayerTarget(game, card, data.GetData<PlayerTarget>(nameof(OwnerId)));
        CardId = data.GetData<int>(nameof(CardId));
    }
}
