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

	private readonly List<ICardAttribute> _attributes = [];

	private PackedScene _objectEditor;

	public List<ICardAttribute> CreateAttributes()
	{
		// TODO: iterate through individual attribute editors and create into a list
		// or can we just return  _attributes.ToList()?
		// what about the existing reference / binding for each attribute?
		// would be best to come up with a new solution here so that references aren't bound to editors.
		return [];
	}

	public void Load(List<ICardAttribute> attributes)
	{
		// TODO: reset and load new attribute list
	}

	public override void _Ready()
	{
		// TODO: Should we use the object editor or the editor registry here?
		_objectEditor = GD.Load<PackedScene>("uid://bxlv4w3wwtsro");

		_newAttributeSelector.Connect(OptionButton.SignalName.ItemSelected, Callable.From<long>(OnNewAttributeSelected));
		_addAttributeButton.Connect(BaseButton.SignalName.Pressed, Callable.From(CreateNewAttribute));
	}

	private void OnNewAttributeSelected(long itemIndex)
	{
		var selectedText = _newAttributeSelector.GetItemText((int)itemIndex);
		_addAttributeButton.Disabled = selectedText == "None" || _attributes.Any(a => a.GetType().Name == selectedText);
	}

	private void CreateNewAttribute()
	{
		var attr = _newAttributeSelector.CreateSelected();
		_attributes.Add(attr);

		CreateAttributeEditor(attr);

		_newAttributeSelector.Select(0);
		OnNewAttributeSelected(0);
	}

	private void Reset()
	{
		// TODO: Reset state
		_attributes.Clear();
		var attributeControls = _attributesContainer.GetChildren();

		foreach (var control in attributeControls)
			control.QueueFree();

		_newAttributeSelector.Select(0);
		OnNewAttributeSelected(0);
	}

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

	private void CreateAttributeEditor(ICardAttribute attr)
	{
		var editor = _objectEditor.Instantiate<ObjectEditor>();
		_attributesContainer.AddChild(editor);

		editor.Load(
			target: attr,
			customTitle: attr.GetType().Name.PrettyPrint().Replace("Attribute", string.Empty)
		);

		// TODO: Re-implement RemovePressed functionality for attributes
		// editor.RemovePressed += () =>
		// {
		// 	_attributesContainer.RemoveChild(editor);
		// 	LoadedData.Attributes.Remove(attr);
		// 	editor.QueueFree();
		// };
	}
}
