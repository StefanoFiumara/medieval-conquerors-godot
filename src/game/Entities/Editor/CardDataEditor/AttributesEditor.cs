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
	[Export] private AttributeOptions _newAttributeSelector;
	[Export] private Button _addAttributeButton;
	[Export] private VBoxContainer _attributesContainer;

	private PackedScene _objectEditor;

	public override void _Ready()
	{
		_objectEditor = GD.Load<PackedScene>("uid://bxlv4w3wwtsro");

		_newAttributeSelector.Connect(OptionButton.SignalName.ItemSelected, Callable.From<long>(OnNewAttributeSelected));
		_addAttributeButton.Connect(BaseButton.SignalName.Pressed, Callable.From(CreateNewAttribute));
	}

	private IEnumerable<ObjectEditor> AttributeEditors => _attributesContainer.GetChildren().OfType<ObjectEditor>();
	public List<ICardAttribute> CreateAttributes()
	{
		return AttributeEditors
			.Select(editor => (ICardAttribute)editor.CreateObject())
			.ToList();
	}

	public void Load(IReadOnlyList<ICardAttribute> attributes)
	{
		Reset();

		foreach (var attr in attributes.OrderBy(attr => attr.GetType().Name))
		{
			var attributeType = attr.GetType();
			AddAttributeEditor(attributeType, source: attr);
		}
	}

	private void OnNewAttributeSelected(long itemIndex)
	{
		var selectedText = _newAttributeSelector.GetItemText((int)itemIndex);
		_addAttributeButton.Disabled = selectedText == "None" || AttributeEditors.Any(e => e.ObjectType.Name == selectedText);
	}

	private void CreateNewAttribute()
	{
		AddAttributeEditor(_newAttributeSelector.SelectedType);
		ClearSelector();
	}

	public void Reset()
	{
		foreach (var control in AttributeEditors)
			control.QueueFree();

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

		var attrEditors = _attributesContainer.GetChildren().OfType<ObjectEditor>();
		foreach (var editor in attrEditors)
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

	private void AddAttributeEditor(Type selectedAttributeType, ICardAttribute source = null)
	{
		// TODO: Add support for custom attribute editors (e.g. ability editor)
		var editor = _objectEditor.Instantiate<ObjectEditor>();
		_attributesContainer.AddChild(editor);

		editor.Load(selectedAttributeType,
			title: $"{selectedAttributeType.Name.Replace("Attribute", string.Empty).PrettyPrint()}",
			source: source,
			allowClose: true);
	}
}
