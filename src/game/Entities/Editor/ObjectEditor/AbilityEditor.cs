using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using MedievalConquerors.Engine.Attributes;
using MedievalConquerors.Entities.Editor.Options;

namespace MedievalConquerors.Entities.Editor;

public partial class AbilityEditor : PanelContainer, IObjectEditor<AbilityAttribute>
{
    // TODO: Set up controls for ability attribute editor
    // 1. List Editor for action definitions
    //     a. Selector for ActionType (populate from list of actions that implement ability loader
    //     b. Depending on selected action, each should have its own object editor to create its parameters.
    //     c. Each editor should be able to serialize its options to a string
    //          i. We can defer this logic to the inner classes


    // NOTE: This is taking a very similar shape to AttributesEditor
    //       perhaps we can combine similar functionality.
    // IDEA: Maybe use IListEditor<T> to abstract out the common functionality that does not rely on the item type.

    // TODO: Get UID for ActionDefinitionEditor
    private static readonly PackedScene _actionEditor = GD.Load<PackedScene>($"uid://invalid");

    private Label _titleLabel;
    private Button _removeButton;

    // TODO: Maybe the action options ought to go inside each action definition editor instead?
    //       Depends on the kind of UX we want
    private GameActionOptions _actionOptions;
    private Button _addActionButton;
    private VBoxContainer _actionsContainer;

    private IEnumerable<IObjectEditor<ActionDefinition>> ActionEditors =>
        _actionsContainer.GetChildren().OfType<IObjectEditor<ActionDefinition>>();

    public override void _Ready()
    {
        _titleLabel = GetNode<Label>("%name_label");
        _removeButton = GetNode<Button>("%close_button");

        _actionOptions = GetNode<GameActionOptions>("%action_options");
        _addActionButton = GetNode<Button>("%add_action_btn");
        _actionsContainer = GetNode<VBoxContainer>("%actions_container");

        _addActionButton.Connect(BaseButton.SignalName.Pressed, Callable.From(CreateNewAction));
        _removeButton.Connect(BaseButton.SignalName.Pressed, Callable.From(QueueFree));
    }

    public void Load(string title, AbilityAttribute source, bool allowDelete = false)
    {
        _titleLabel.Text = title ?? string.Empty;
        _removeButton.Visible = allowDelete;

        foreach (var action in source.Actions)
            AddActionEditor(action);

    }

    private void AddActionEditor(ActionDefinition action)
    {
        var editor = _actionEditor.Instantiate<ActionDefinitionEditor>();
        _actionsContainer.AddChild(editor);
    }

    public AbilityAttribute Create()
    {
        throw new NotImplementedException();
    }

    private void CreateNewAction()
    {
        // TODO: Create child action definition editor scene based on selected action
        throw new NotImplementedException();
    }

    public void Enable()
    {
        // TODO: Loop through all children in container and enable
        throw new NotImplementedException();
    }

    public void Disable()
    {
        // TODO: Loop through all children in container and disable
        throw new NotImplementedException();
    }

    public Control GetControl() => this;
}
