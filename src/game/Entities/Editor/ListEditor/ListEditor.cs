using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using MedievalConquerors.Entities.Editor.ValueEditors;
using MedievalConquerors.Extensions;

namespace MedievalConquerors.Entities.Editor;

public partial class ListEditor<T> : PanelContainer, IListEditor<T>
    where T : class
{
    private Label _titleLabel;
    private Button _removeButton;

    private TypeOptions<T> _typeSelector;
    private VBoxContainer _editorsContainer;
    private Button _addButton;

    private IEnumerable<IObjectEditor<T>> Editors =>
        _editorsContainer.GetChildren() .OfType<IObjectEditor<T>>();

    public override void _Ready()
    {
        _titleLabel = GetNode<Label>("%title_label");
        _removeButton = GetNode<Button>("%close_button");

        _addButton = GetNode<Button>("%add_item_btn");
        _editorsContainer = GetNode<VBoxContainer>("%editors_container");

        _typeSelector = CreateTypeSelector();
        if (_typeSelector != null)
        {
            var controls = GetNode<HBoxContainer>("%editor_controls");
            controls.AddChild(_typeSelector);
            controls.MoveChild(_typeSelector, 2);
            _typeSelector.Connect(OptionButton.SignalName.ItemSelected, Callable.From<long>(OnNewTypeSelected));
        }

        _addButton.Connect(BaseButton.SignalName.Pressed, Callable.From(CreateNewEditor));
    }

    private TypeOptions<T> CreateTypeSelector()
    {
        // TODO: Dynamically create type selector
        //         - Find all inheritors of type options
        //         - Check if any generic parameters are a match for TEditorValue
        //         - Create that instance
        throw new NotImplementedException();
    }

    public void Load(string title, List<T> source, bool allowDelete = false)
    {
        Reset();
        _titleLabel.Text = title ?? string.Empty;
        _removeButton.Visible = allowDelete;

        foreach (var item in source.OrderBy(t => t.GetType().Name))
            AddNewEditor(item);
    }

    public void Reset()
    {
        foreach (var editor in Editors)
            editor.GetControl().QueueFree();
    }

    public void Enable()
    {
        _typeSelector?.Disabled = false;
        _addButton.Disabled = _typeSelector?.Selected == 0;

        foreach (var editor in Editors)
            editor.Enable();
    }

    public void Disable()
    {
        _typeSelector?.Disabled = true;
        _addButton.Disabled = true;

        foreach (var editor in Editors)
            editor.Disable();
    }

    public List<T> Create() => Editors.Select(e => e.Create()).ToList();
    public Control GetControl() => this;

    private void CreateNewEditor()
    {
        var instance = _typeSelector == null
                ? Activator.CreateInstance<T>()
                : (T) Activator.CreateInstance(_typeSelector.SelectedType);

        AddNewEditor(instance);
        ClearSelector();
    }

    private void AddNewEditor(T source)
    {
        var editor = EditorFactory.CreateObjectEditor(typeof(T));
        _editorsContainer.AddChild(editor.GetControl());
        editor.Load(title: $"{typeof(T).Name.Replace("Attribute", string.Empty).PrettyPrint()}",
            source: source,
            allowDelete: true);
    }

    private void ClearSelector()
    {
        if (_typeSelector == null) return;

        _typeSelector.Select(0);
        OnNewTypeSelected(0);
    }

    private void OnNewTypeSelected(long itemIndex)
    {
        if (_typeSelector == null) return;

        var selectedText = _typeSelector.GetItemText((int)itemIndex);
        _addButton.Disabled = selectedText == "None" || Editors.Any(e => e.ObjectType.Name == selectedText);
    }
}
