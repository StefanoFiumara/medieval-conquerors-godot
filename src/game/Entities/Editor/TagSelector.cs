using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using MedievalConquerors.Engine.Data;
using MedievalConquerors.Entities.Editor.ValueEditors;

namespace MedievalConquerors.Entities.Editor;

public partial class TagSelector : GridContainer, IValueEditor
{
	[Signal]
	public delegate void TagsChangedEventHandler();

	private readonly Dictionary<Tags, CheckBox> _tagSelectors = new();

	private Tags _selectedTags = Tags.None;
	public Tags SelectedTags
	{
		get => _selectedTags;
		set
		{
			if (_selectedTags != value)
			{
				_selectedTags = value;
				if (_tagSelectors is { Count: > 0 })
				{
					foreach (var tagSelector in _tagSelectors)
						tagSelector.Value.SetPressedNoSignal(value.HasFlag(tagSelector.Key));
				}
				EmitSignal(SignalName.TagsChanged);
			}
		}
	}

	public override void _EnterTree()
	{
		_tagSelectors.Clear();

		foreach (var tag in Enum.GetValues<Tags>().Skip(1))
		{
			var checkBox = new CheckBox();
			checkBox.Text = tag.ToString();
			_tagSelectors.Add(tag, checkBox);
			AddChild(checkBox);
			checkBox.Connect(BaseButton.SignalName.Toggled, Callable.From<bool>(_ =>
			{
				if (_tagSelectors is { Count: > 0 })
				{
					SelectedTags = _tagSelectors
						.Where(s => s.Value.ButtonPressed)
						.Aggregate(Tags.None, (current, selector) => current | selector.Key);
				}
			}));
		}

		foreach (var tagSelector in _tagSelectors)
			tagSelector.Value.ButtonPressed = _selectedTags.HasFlag(tagSelector.Key);
	}

	public void Enable()
	{
		foreach (var checkBox in _tagSelectors.Values)
			checkBox.Disabled = false;
	}

	public void Disable()
	{
		foreach (var checkBox in _tagSelectors.Values)
			checkBox.Disabled = true;
	}

	public Control GetControl() => this;
	public object GetValue() => SelectedTags;
}
