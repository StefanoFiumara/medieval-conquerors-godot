using System.Collections;
using Godot;
using MedievalConquerors.Extensions;

namespace MedievalConquerors.Entities.Editor;

public partial class ListItem : MarginContainer
{
	private Button _removeButton;
	private IList _owner;
	private CenterContainer _itemContainer;

	public override void _Ready()
	{
		_removeButton = GetNode<Button>("%remove_button");
		_itemContainer = GetNode<CenterContainer>("%item_container");
	}

	public void Load(IList owner, object item)
	{
		_owner = owner;
		_removeButton.Connect(BaseButton.SignalName.Pressed, Callable.From(() =>
		{
			_owner.Remove(item);
			QueueFree();
		}));

		var editor = EditorFactory.CreateEditor(item, $"[{owner.IndexOf(item)}] {item.GetType().Name.PrettyPrint()}");
		_itemContainer.AddChild(editor.GetControl());
	}
}
