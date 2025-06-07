using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using MedievalConquerors.Engine.Data;

namespace MedievalConquerors.Entities.Editor;

public partial class TagSelector : GridContainer
{
	private readonly Dictionary<Tags, CheckBox> _tagSelectors = new();

	[Signal]
	public delegate void TagsChangedEventHandler();

	public Tags SelectedTags
	{
		get
		{
			if (_tagSelectors == null) return Tags.None;

			return _tagSelectors
				.Where(s => s.Value.ButtonPressed)
				.Aggregate(Tags.None, (current, selector) => current | selector.Key);
		}
		set
		{
			if (_tagSelectors == null) return;

			var selected = SelectedTags;
			if (selected != value)
			{
				foreach (var tagSelector in _tagSelectors)
				{
					tagSelector.Value.ButtonPressed = value.HasFlag(tagSelector.Key);
				}
				EmitSignal(nameof(TagsChangedEventHandler));
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
			checkBox.Toggled += OnTagsChanged;
		}
	}

	private void OnTagsChanged(bool toggled)
	{
		EmitSignal(nameof(TagsChangedEventHandler));
	}

	public void Enable()
	{
		foreach (var checkBox in _tagSelectors.Values)
		{
			checkBox.Disabled = false;
		}
	}

	public void Disable()
	{
		foreach (var checkBox in _tagSelectors.Values)
		{
			checkBox.Disabled = true;
		}
	}

	public override void _ExitTree()
	{
		if (_tagSelectors != null)
		{
			foreach (var checkBox in _tagSelectors.Values)
			{
				checkBox.Toggled -= OnTagsChanged;
				checkBox.QueueFree();
			}

			_tagSelectors.Clear();
		}
	}
}
