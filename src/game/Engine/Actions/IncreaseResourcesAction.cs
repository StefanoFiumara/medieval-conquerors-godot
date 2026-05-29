using System;
using System.Collections.Generic;
using Godot;
using MedievalConquerors.Editors;
using MedievalConquerors.Editors.Options;
using MedievalConquerors.Engine.Attributes;
using MedievalConquerors.Engine.Core;
using MedievalConquerors.Engine.Data;

namespace MedievalConquerors.Engine.Actions;

public class IncreaseResourcesAction(int targetPlayerId, int food, int wood, int gold, int stone) : GameAction, IAbilityLoader
{
    [UseValueEditor(typeof(PlayerTargetOptions))]
    public int TargetPlayerId { get; private set; } = targetPlayerId;

    public int Food { get; private set; }  = food;
    public int Wood { get; private set; }  = wood;
    public int Gold { get; private set; }  = gold;
    public int Stone { get; private set; } = stone;

    public IncreaseResourcesAction() : this(-1, 0,0,0,0) { }

    public static Dictionary<string, Type> GetParameters() =>
        new()
        {
            { nameof(TargetPlayerId), typeof(PlayerTarget) },
            { nameof(Food), typeof(int) },
            { nameof(Wood), typeof(int) },
            { nameof(Gold), typeof(int) },
            { nameof(Stone), typeof(int) },
        };

    public void Load(IGame game, Card card, AbilityAttribute ability, ActionDefinition data, Vector2I targetTile)
    {
        TargetPlayerId = this.ResolvePlayerTarget(game, card, data.GetData<PlayerTarget>(nameof(TargetPlayerId)));
        Food = data.GetData<int>(nameof(Food));
        Wood = data.GetData<int>(nameof(Wood));
        Gold = data.GetData<int>(nameof(Gold));
        Stone = data.GetData<int>(nameof(Stone));
    }
}
