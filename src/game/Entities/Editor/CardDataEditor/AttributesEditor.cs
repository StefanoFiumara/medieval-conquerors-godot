using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using MedievalConquerors.Engine.Data;
using MedievalConquerors.Entities.Editor.Options;
using AttributeOptions = MedievalConquerors.Entities.Editor.ValueEditors.AttributeOptions;

namespace MedievalConquerors.Entities.Editor;

public partial class AttributesEditor : PanelContainer
{
	[Export] private AttributeOptions _newAttributeSelector;
	[Export] private Button _addAttributeButton;
	[Export] private VBoxContainer _attributesContainer;

	private PackedScene _objectEditor;

	public List<ICardAttribute> CreateAttributes()
	{
		// TODO: iterate through individual attribute editors and create into a list
		return [];
	}

	public void Load(IReadOnlyList<ICardAttribute> attributes)
	{
		// TODO: reset and load new attribute list
	}

	public override void _Ready()
	{
		_objectEditor = GD.Load<PackedScene>("uid://bxlv4w3wwtsro");

		_newAttributeSelector.Connect(OptionButton.SignalName.ItemSelected, Callable.From<long>(OnNewAttributeSelected));
		_addAttributeButton.Connect(BaseButton.SignalName.Pressed, Callable.From(CreateNewAttribute));
	}

	private void OnNewAttributeSelected(long itemIndex)
	{
		var selectedText = _newAttributeSelector.GetItemText((int)itemIndex);
		// TODO: Check if the selected attribute exists in the list to disable the button - no duplicates!
		_addAttributeButton.Disabled = selectedText == "None";
	}

	private void CreateNewAttribute()
	{
		var attributeType = _newAttributeSelector.GetSelectedType();

		CreateAttributeEditor(attributeType);

		_newAttributeSelector.Select(0);
		OnNewAttributeSelected(0);
	}

	public void Reset()
	{
		var attributeControls = _attributesContainer.GetChildren();
		foreach (var control in attributeControls)
			control.QueueFree();

		_newAttributeSelector.Select(0);
		OnNewAttributeSelected(0);
	}

	// TODO: enable/disable attribute editors?
	public void Enable()
	{
		_newAttributeSelector.Disabled = false;
		_addAttributeButton.Disabled = _newAttributeSelector.Selected == 0;
	}

	public void Disable()
	{
		_newAttributeSelector.Disabled = true;
		_addAttributeButton.Disabled = true;
	}

	private void CreateAttributeEditor(Type selectedAttributeType)
	{
		// TODO: should there be a registry for this so we can use other editors? e.g. for the ability attribute?
		//		Should they share the same IValueEditor interface, or another?
		var editor = _objectEditor.Instantiate<ObjectEditor>();
		_attributesContainer.AddChild(editor);
		editor.Load(selectedAttributeType);

		// TODO: Re-implement attribute removal
		// editor.RemovePressed += () =>
		// {
		// 	_attributesContainer.RemoveChild(editor);
		// 	LoadedData.Attributes.Remove(attr);
		// 	editor.QueueFree();
		// };
	}
}
