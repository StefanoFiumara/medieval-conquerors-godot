using System;
using System.Linq;
using Godot;
using MedievalConquerors.ConquerorsPlugin.Controls;
using MedievalConquerors.ConquerorsPlugin.Data;
using MedievalConquerors.Engine.Data;

namespace MedievalConquerors.ConquerorsPlugin.LibraryBrowser;

[Tool]
public partial class CardLibrary : ScrollContainer
{
	[Export] private Button _clearButton;
	[Export] private LineEdit _searchInput;
	[Export] private TagOptions _tagFilter;
	[Export] private CardTypeOptions _typeFilter;
	[Export] private GridContainer _resultsContainer;
	
	private PackedScene _searchResultScene;
	
	public event Action<CardData> SearchResultClicked;
	
	public override void _Ready()
	{
		_searchResultScene = GD.Load<PackedScene>("res://addons/ConquerorsPlugin/LibraryBrowser/card_search_result.tscn");
	}

	public override void _EnterTree()
	{
		_clearButton.Pressed += ClearSearch;
		_searchInput.TextChanged += SearchTextChanged;
		_tagFilter.TagsChanged += UpdateResults;
		_typeFilter.ItemSelected += TypeFilterOnItemSelected;
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
		ClearResults();
		
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
		
		// NOTE: run enum filter in-memory, LiteDB has trouble with bitwise operators
		if (_tagFilter.SelectedTags != Tags.None)
			results = results.Where(c => c.Tags.HasFlag(_tagFilter.SelectedTags)).ToList();
		
		foreach (var card in results)
		{
			var cardResult = _searchResultScene.Instantiate<CardSearchResult>();
			_resultsContainer.AddChild(cardResult);
			cardResult.Load(card);
			cardResult.OnSelected += SendToEditor;
		}
	}

	private void ClearResults()
	{
		foreach (var node in _resultsContainer.GetChildren())
		{
			_resultsContainer.RemoveChild(node);
			node.QueueFree();
		}
	}
	
	private void SendToEditor(CardData card)
	{
		SearchResultClicked?.Invoke(card);
	}

	public override void _ExitTree()
	{
		_clearButton.Pressed -= ClearSearch;
		_searchInput.TextChanged -= SearchTextChanged;
		_tagFilter.TagsChanged -= UpdateResults;
		_typeFilter.ItemSelected -= TypeFilterOnItemSelected;
	}
}
