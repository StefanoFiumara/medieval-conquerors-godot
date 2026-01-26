using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using MedievalConquerors.Engine.Data;
using MedievalConquerors.Entities.Editor.Options;
using MedievalConquerors.Extensions;

namespace MedievalConquerors.Entities.Editor;

public partial class AttributesEditor : PanelContainer
{
	private static readonly PackedScene _objectEditor = GD.Load<PackedScene>("uid://bxlv4w3wwtsro");

	// TODO: Wire these up in _Ready instead of relying on [Export]
	[Export] private AttributeOptions _newAttributeSelector;
	[Export] private Button _addAttributeButton;
	[Export] private VBoxContainer _attributesContainer;

	private IEnumerable<IObjectEditor> AttributeEditors =>
		_attributesContainer.GetChildren().OfType<IObjectEditor>();

	public override void _Ready()
	{
		_newAttributeSelector.Connect(OptionButton.SignalName.ItemSelected, Callable.From<long>(OnNewAttributeSelected));
		_addAttributeButton.Connect(BaseButton.SignalName.Pressed, Callable.From(CreateNewAttribute));
	}

	public void Load(IReadOnlyList<ICardAttribute> attributes)
	{
		Reset();

		foreach (var attr in attributes.OrderBy(attr => attr.GetType().Name))
			AddAttributeEditor(attr);
	}

	public List<ICardAttribute> CreateAttributes()
	{
		return AttributeEditors
			.Select(editor => (ICardAttribute)editor.Create())
			.ToList();
	}

	private void OnNewAttributeSelected(long itemIndex)
	{
		var selectedText = _newAttributeSelector.GetItemText((int)itemIndex);
		_addAttributeButton.Disabled = selectedText == "None" || AttributeEditors.Any(e => e.ObjectType.Name == selectedText);
	}

	private void CreateNewAttribute()
	{
		var attribute = (ICardAttribute)Activator.CreateInstance(_newAttributeSelector.SelectedType);
		AddAttributeEditor(attribute);
		ClearSelector();
	}

	public void Reset()
	{
		foreach (var editor in AttributeEditors)
			editor.GetControl().QueueFree();

		ClearSelector();
	}

	private void ClearSelector()
	{
		_newAttributeSelector.Select(0);
		OnNewAttributeSelected(0);
	}

	public void Enable()
	{
		_newAttributeSelector.Disabled = false;
		_addAttributeButton.Disabled = _newAttributeSelector.Selected == 0;

		foreach (var editor in AttributeEditors)
			editor.Enable();
	}

	public void Disable()
	{
		_newAttributeSelector.Disabled = true;
		_addAttributeButton.Disabled = true;

		var attrEditors = _attributesContainer.GetChildren().OfType<ObjectEditor>();
		foreach (var editor in attrEditors)
			editor.Disable();
	}

	private void AddAttributeEditor(ICardAttribute source)
	{
		// TODO: Add support for custom attribute editors (e.g. ability editor)
		// IDEA: can this registry be incorporated in EditorFactory?
		var editor = _objectEditor.Instantiate<ObjectEditor>();
		_attributesContainer.AddChild(editor);

		var type = source.GetType();
		editor.Load(title: $"{type.Name.Replace("Attribute", string.Empty).PrettyPrint()}",
			source: source,
			allowDelete: true);
	}
}
