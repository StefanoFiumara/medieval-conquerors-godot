using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using MedievalConquerors.Entities.Editor.Options;
using MedievalConquerors.Extensions;

namespace MedievalConquerors.Entities.Editor;

public partial class ListEditor<TEditorValue> : PanelContainer, IObjectEditor<List<TEditorValue>>
    where TEditorValue : class
{
    private Label _titleLabel;
    private Button _removeButton;

    // TODO: Will TypeOptions work here since it is abstract? how do we get a reference to the proper type?
    //       We won't be able to hook this up directly from the scene, it will have to be added dynamically
    //       So maybe we need a reference to its container instead.
    private TypeOptions<TEditorValue> _typeSelector;
    private VBoxContainer _editorsContainer;
    private Button _addButton;

    private IEnumerable<IObjectEditor<TEditorValue>> Editors =>
        _editorsContainer.GetChildren() .OfType<IObjectEditor<TEditorValue>>();

    public override void _Ready()
    {
        _titleLabel = GetNode<Label>("%title_label");
        _removeButton = GetNode<Button>("%close_button");

        _addButton = GetNode<Button>("%add_item_btn");
        _editorsContainer = GetNode<VBoxContainer>("%editors_container");

        // TODO: Dynamically create type selector and add to scene
        // IDEA: Maybe we create type selector only when we have a definition for TypeOptions<TEditorValue>
        //       And we can default to create instances of TEditorValue with Activator.CreateInstance otherwise.
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

    private TypeOptions<TEditorValue> CreateTypeSelector()
    {
        // TODO: Find all inheritors of type options
        //       Check if any generic parameters are a match for TEditorValue
        //       Create that instance
        throw new NotImplementedException();
    }

    public void Load(string title, List<TEditorValue> source, bool allowDelete = false)
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
        if(_typeSelector != null)
            _typeSelector.Disabled = false;

        _addButton.Disabled = _typeSelector?.Selected == 0;

        foreach (var editor in Editors)
            editor.Enable();
    }

    public void Disable()
    {
        if(_typeSelector != null)
            _typeSelector.Disabled = true;

        _addButton.Disabled = true;

        foreach (var editor in Editors)
            editor.Disable();
    }

    public List<TEditorValue> Create() => Editors.Select(e => e.Create()).ToList();
    public Control GetControl() => this;

    private void CreateNewEditor()
    {
        var instance = _typeSelector == null
                ? Activator.CreateInstance<TEditorValue>()
                : (TEditorValue) Activator.CreateInstance(_typeSelector.SelectedType);

        AddNewEditor(instance);
        ClearSelector();
    }

    private void AddNewEditor(TEditorValue source)
    {
        var editor = EditorFactory.CreateObjectEditor(typeof(TEditorValue));
        _editorsContainer.AddChild(editor.GetControl());
        editor.Load(title: $"{typeof(TEditorValue).Name.Replace("Attribute", string.Empty).PrettyPrint()}",
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
