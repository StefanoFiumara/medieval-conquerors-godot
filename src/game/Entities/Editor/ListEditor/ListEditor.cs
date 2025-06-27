using System;
using System.Collections;
using Godot;

namespace MedievalConquerors.Entities.Editor;

/// <summary>
/// ListEditor is a generic Godot editor panel for displaying and editing the items of any List<T>.
/// It creates an ObjectEditor for each item and allows adding/removing items.
/// </summary>
public partial class ListEditor : PanelContainer
{
	private Button _addButton;
	private VBoxContainer _itemsContainer;
	private PackedScene _objectEditorScene;

	private IList _list;
	private Type _itemType;

	public override void _Ready()
	{
		// TODO: Create Scene for ListEditor
		_addButton = GetNode<Button>("%add_button");
		_itemsContainer = GetNode<VBoxContainer>("%items_container");
		// TODO: double check UID
		_objectEditorScene = GD.Load<PackedScene>("uid://bxlv4w3wwtsro");

		_addButton.Pressed += OnAddPressed;
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

		foreach (var child in _itemsContainer.GetChildren())
			child.QueueFree();

		foreach (var item in list)
			AddItemEditor(item);
	}

	private void AddItemEditor(object item)
	{
		var editor = _objectEditorScene.Instantiate<ObjectEditor>();
		_itemsContainer.AddChild(editor);
		editor.Load(item);
		
		var removeButton = new Button { Text = "Remove" };
		
		var listItem = new HBoxContainer();
		listItem.AddChild(removeButton);
		listItem.AddChild(editor);
		_itemsContainer.AddChild(listItem);

		removeButton.Connect(BaseButton.SignalName.Pressed, Callable.From(() => RemoveItem(listItem, item)));
	}

	private void RemoveItem(HBoxContainer container, object item)
	{
		_itemsContainer.RemoveChild(container);
		_list.Remove(item);
		container.QueueFree();
	}

	private void OnAddPressed()
	{
		// TODO: Support for polymorphic lists
		// For polymorphic lists, we may want to provide a factory or selector for the type
		var newItem = Activator.CreateInstance(_itemType);
		_list.Add(newItem);
		AddItemEditor(newItem);
	}
}
