using System;
using System.Collections;
using Godot;

namespace MedievalConquerors.Entities.Editor;

/// <summary>
/// ListEditor is a generic Godot editor panel for displaying and editing the items of a List.
/// It creates an ObjectEditor for each item and allows adding/removing items.
/// </summary>
public partial class ListEditor : PanelContainer
{
	private Button _addButton;
	private VBoxContainer _itemsContainer;
	private PackedScene _itemEditor;

	private IList _list;
	private Type _itemType;

	public override void _Ready()
	{
		_itemEditor = GD.Load<PackedScene>("uid://bcdq665qxr1e8");
		_addButton = GetNode<Button>("%add_button");
		_itemsContainer = GetNode<VBoxContainer>("%items_container");

		_addButton.Connect(BaseButton.SignalName.Pressed, Callable.From(OnAddPressed));
	}

	/// <summary>
	/// Loads the editor with the items of the specified list.
	/// </summary>
	/// <param name="list">The list to edit.</param>
	/// <param name="itemType">The type of items in the list.</param>
	/// <param name="customTitle">Optional. A custom title for the editor panel.</param>
	public void Load(IList list, Type itemType, string customTitle = null)
	{
		_list = list;
		_itemType = itemType;

		// TODO: This doesn't work because CreateItemEditor is run before _Ready, how to handle this?
		foreach (var item in list)
			CreateItemEditor(item);
	}

	private void CreateItemEditor(object item)
	{
		var editor = _itemEditor.Instantiate<ListItem>();
		_itemsContainer.AddChild(editor);
		editor.Load(_list, item);

	}

	private void OnAddPressed()
	{
		// TODO: Support for polymorphic lists
		// For polymorphic lists, we may want to provide a factory or selector for the type
		var newItem = Activator.CreateInstance(_itemType);
		_list.Add(newItem);
		CreateItemEditor(newItem);
	}
}
