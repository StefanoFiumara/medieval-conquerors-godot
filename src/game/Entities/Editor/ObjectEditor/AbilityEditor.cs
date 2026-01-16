using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using MedievalConquerors.Engine.Attributes;
using MedievalConquerors.Entities.Editor.Options;
using MedievalConquerors.Extensions;

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

    private GameActionOptions _actionOptions;
    private Button _addActionButton;
    private VBoxContainer _actionsContainer;

    private Type _abilityType;

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
        _abilityType = source.GetType();
        _titleLabel.Text = title ?? string.Empty;
        _removeButton.Visible = allowDelete;

        foreach (var action in source.Actions)
            AddActionEditor(action);

    }

    private void AddActionEditor(ActionDefinition action)
    {
        var editor = _actionEditor.Instantiate<ActionDefinitionEditor>();
        editor.Load(action.ActionType.PrettyPrint(), action, allowDelete: true);
        _actionsContainer.AddChild(editor);
    }

    private void CreateNewAction() => AddActionEditor(new ActionDefinition
    {
        ActionType = _actionOptions.SelectedType.FullName
    });

    public AbilityAttribute Create()
    {
        var attr = (AbilityAttribute) Activator.CreateInstance(_abilityType);
        return attr! with
        {
            Actions = ActionEditors.Select(e => e.Create()).ToList()
        };
    }

    public void Enable()
    {
        foreach (var editor in _actionsContainer.GetChildren().OfType<ActionDefinitionEditor>()) 
            editor.Enable();
    }

    public void Disable()
    {
        foreach (var editor in _actionsContainer.GetChildren().OfType<ActionDefinitionEditor>()) 
            editor.Enable();
    }

    public Control GetControl() => this;
}
