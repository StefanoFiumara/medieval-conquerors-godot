using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Godot;
using MedievalConquerors.Entities.Editor.ValueEditors;
using MedievalConquerors.Extensions;

namespace MedievalConquerors.Entities.Editor;

/// <summary>
/// ListEditor is a generic Godot editor panel for displaying and editing the items of a List.
/// It creates an ObjectEditor for each item and allows adding/removing items.
/// </summary>
public partial class ListEditor : PanelContainer, IPropertyEditor, IRootEditor
{
	private Button _addButton;
	private VBoxContainer _itemsContainer;
	private PackedScene _itemEditor;

	private IList _list;
	private Type _itemType;
	private string _title;

	public override void _Ready()
	{
		_itemEditor = GD.Load<PackedScene>("uid://bcdq665qxr1e8");
		_addButton = GetNode<Button>("%add_button");
		_itemsContainer = GetNode<VBoxContainer>("%items_container");

		_addButton.Connect(BaseButton.SignalName.Pressed, Callable.From(OnAddPressed));
	}

	public void Load(object target, string title = null, Func<PropertyInfo, bool> propertyFilter = null)
	{
		var type = target.GetType();
		if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
			Load(target as IList, target.GetType().GetGenericArguments()[0], title);
		else
			GD.PrintErr($"{nameof(ListEditor)} -- Passed non-list argument to Load()");
	}

	public void Load<TOwner>(TOwner owner, PropertyInfo prop)
	{
		if (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(List<>))
			Load(prop.GetValue(owner) as IList, prop.PropertyType.GetGenericArguments()[0], prop.Name.PrettyPrint());
		else
			GD.PrintErr($"{nameof(ListEditor)} -- Passed non-list argument to Load()");
	}

	private void Load(IList list, Type itemType, string title = null)
	{
		_list = list;
		_itemType = itemType;
		_title = title;

		// TODO: Check if item type is an interface, if so, provide a selector for all types deriving from that interface
		//		 So that the user can dynamically select which object to create when clicking the Add button
		CallDeferred(MethodName.LoadDeferred);
	}

	private void LoadDeferred()
	{
		// TODO: Set title
		foreach (var item in _list)
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

	public Control GetControl() => this;
}
