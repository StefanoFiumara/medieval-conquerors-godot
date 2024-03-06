using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using MedievalConquerors.Engine.Data;

namespace MedievalConquerors.addons.CardDataEditor.Controls;

[Tool]
public partial class TagOptions : GridContainer
{
	private Dictionary<Tags, CheckBox> _tagSelectors;
	private List<Tags> _tagOptions;
	private Tags _selectedTags;

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
			_selectedTags = value;
			if (_tagSelectors == null) return;
			
			foreach (var tagSelector in _tagSelectors)
			{
				tagSelector.Value.ButtonPressed = _selectedTags.HasFlag(tagSelector.Key);
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
		}
	}
}
