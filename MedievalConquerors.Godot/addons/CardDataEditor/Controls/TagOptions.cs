using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using MedievalConquerors.Engine.Data;

namespace MedievalConquerors.Addons.CardDataEditor.Controls;

[Tool]
public partial class TagOptions : GridContainer
{
	private Dictionary<Tags, CheckBox> _tagSelectors;
	private List<Tags> _tagOptions;
	private Tags _selectedTags;
	public event Action TagsChanged;

	public Tags SelectedTags
	{
		get
		{
			if (_tagSelectors == null) return Tags.None;

			var checkedBoxes = _tagSelectors
				.Where(s => s.Value.ButtonPressed)
				.ToList();
			
			var result = checkedBoxes
				.Aggregate(Tags.None, (current, selector) => current | selector.Key);
			
			return result;
		}
		set
		{
			if (_tagSelectors == null) return;
			
			if (_selectedTags != value)
			{
				foreach (var tagSelector in _tagSelectors)
				{
					tagSelector.Value.ButtonPressed = _selectedTags.HasFlag(tagSelector.Key);
				}
				
				_selectedTags = value;
				TagsChanged?.Invoke();
			}
		}
	}

	public override void _Ready()
	{
		_tagSelectors = new();
		_tagOptions = Enum.GetValues<Tags>().Skip(1).ToList();

		foreach (var tag in _tagOptions)
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
		TagsChanged?.Invoke();
	}

	public void Disable()
	{
		foreach (var checkBox in _tagSelectors.Values)
		{
			checkBox.Disabled = true;
		}
	}
	
	public void Enable()
	{
		foreach (var checkBox in _tagSelectors.Values)
		{
			checkBox.Disabled = false;
		}
	}

	public override void _ExitTree()
	{
		if (_tagSelectors != null)
		{
			foreach (var checkBox in _tagSelectors.Values)
			{
				checkBox.Toggled -= OnTagsChanged;
			}
		}
	}
}
