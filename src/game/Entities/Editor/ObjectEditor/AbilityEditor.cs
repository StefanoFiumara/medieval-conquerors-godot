using System;
using Godot;
using MedievalConquerors.Engine.Attributes;

namespace MedievalConquerors.Entities.Editor;

public partial class AbilityEditor : PanelContainer, IObjectEditor<AbilityAttribute>
{
    public override void _Ready()
    {
        // TODO: Set up controls for ability attribute editor
    }

    public void Load(string title, AbilityAttribute source, bool allowClose = false)
    {
        throw new NotImplementedException();
    }

    public AbilityAttribute Create()
    {
        throw new NotImplementedException();
    }

    public void Enable()
    {
        throw new NotImplementedException();
    }

    public void Disable()
    {
        throw new NotImplementedException();
    }

    public Control GetControl() => this;
}
