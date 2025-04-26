using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using MedievalConquerors.Engine.Data;

namespace MedievalConquerors.entities.editor.library_editor;

public partial class LibraryEditor : ScrollContainer
{
	[Export] private Button _clearButton;
	[Export] private LineEdit _searchInput;
	[Export] private TagOptions _tagFilter;
	[Export] private CardTypeOptions _typeFilter;
	[Export] private ItemList _resultsContainer;

	public event Action<CardData> SearchResultClicked;

	private readonly List<CardData> _searchResults = new();

	public override void _EnterTree()
	{
		_clearButton.Pressed += ClearSearch;
		_searchInput.TextChanged += SearchTextChanged;
		_tagFilter.TagsChanged += UpdateResults;
		_typeFilter.ItemSelected += TypeFilterOnItemSelected;
		_resultsContainer.ItemSelected += SendToEditor;
	}

	private void ClearSearch()
	{
		_searchInput.TextChanged -= SearchTextChanged;
		_tagFilter.TagsChanged -= UpdateResults;
		_typeFilter.ItemSelected -= TypeFilterOnItemSelected;

		_searchInput.Text = string.Empty;
		_tagFilter.SelectedTags = Tags.None;
		_typeFilter.SelectedOption = CardType.None;

		_searchInput.TextChanged += SearchTextChanged;
		_tagFilter.TagsChanged += UpdateResults;
		_typeFilter.ItemSelected += TypeFilterOnItemSelected;

		UpdateResults();
	}

	private void TypeFilterOnItemSelected(long index) => UpdateResults();

	private void SearchTextChanged(string text) => UpdateResults();

	private void UpdateResults()
	{
		using var database = new CardDatabase();
		var query = database.Query;

		if (_typeFilter.SelectedOption != CardType.None)
			query = query.Where(c => c.CardType == _typeFilter.SelectedOption);

		if (_searchInput.Text.Length > 2)
		{
			query = query.Where(c =>
				c.Title.Contains(_searchInput.Text, StringComparison.OrdinalIgnoreCase) ||
				c.Description.Contains(_searchInput.Text, StringComparison.OrdinalIgnoreCase));
		}

		var results = query.OrderBy(c => c.CardType).ToList();

		// NOTE: we're running the tag filter in-memory, LiteDB has trouble with bitwise operators
		if (_tagFilter.SelectedTags != Tags.None)
			results = results.Where(c => c.Tags.HasFlag(_tagFilter.SelectedTags)).ToList();

		_resultsContainer.Clear();
		_searchResults.Clear();
		foreach (var card in results)
		{
			_searchResults.Add(card);

			var iconPath = ResourceUid.TextToId(card.ImagePath) == ResourceUid.InvalidId
				? "uid://dnofkwp8xw7ys" // Missing Icon
				: card.ImagePath;

			var icon = GD.Load<Texture2D>(iconPath);
			_resultsContainer.AddItem(card.Title, icon);
		}
	}

	private void SendToEditor(long selectedIndex)
	{
		SearchResultClicked?.Invoke(_searchResults[(int)selectedIndex]);
	}

	public override void _ExitTree()
	{
		_clearButton.Pressed -= ClearSearch;
		_searchInput.TextChanged -= SearchTextChanged;
		_tagFilter.TagsChanged -= UpdateResults;
		_typeFilter.ItemSelected -= TypeFilterOnItemSelected;
		_resultsContainer.ItemSelected -= SendToEditor;
	}
}
