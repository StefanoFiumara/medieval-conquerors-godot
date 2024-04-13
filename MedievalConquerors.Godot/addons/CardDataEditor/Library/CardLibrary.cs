using System;
using System.Linq;
using Godot;
using MedievalConquerors.Addons.CardDataEditor;
using MedievalConquerors.Addons.CardDataEditor.Controls;
using MedievalConquerors.Engine.Data;
using MedievalConquerors.Extensions;

namespace MedievalConquerors.addons.CardDataEditor.Library;

[Tool]
public partial class CardLibrary : ScrollContainer
{
	private Button _clearButton;
	private LineEdit _searchInput;
	private TagOptions _tagFilter;
	private CardTypeOptions _typeFilter;
	
	private GridContainer _resultsContainer;
	private PackedScene _searchResultScene;
	

	public event Action<CardData> SearchResultClicked;
	public override void _Ready()
	{
		_searchResultScene = GD.Load<PackedScene>("res://addons/CardDataEditor/Library/search_card_result.tscn");
			
		_clearButton = GetNode<Button>("%clear_btn");
		_searchInput = GetNode<LineEdit>("%search_input");
		_tagFilter = GetNode<TagOptions>("%tag_filter");
		_typeFilter = GetNode<CardTypeOptions>("%type_filter");
		_resultsContainer = GetNode<GridContainer>("%results_container");
		
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
		_typeFilter.SelectedCardType = CardType.None;
		
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
		
		if (_typeFilter.SelectedCardType != CardType.None)
			query = query.Where(c => c.CardType == _typeFilter.SelectedCardType);

		if (_searchInput.Text.Length > 2)
		{
			query = query.Where(c =>
				c.Title.Contains(_searchInput.Text, StringComparison.OrdinalIgnoreCase) ||
				c.Description.Contains(_searchInput.Text, StringComparison.OrdinalIgnoreCase));
		}

		var results = query.ToList();
		
		// NOTE: run enum filter in-memory, LiteDB has trouble with bitwise operators
		if (_tagFilter.SelectedTags != Tags.None)
			results = results.Where(c => c.Tags.HasFlag(_tagFilter.SelectedTags)).ToList();
		
		foreach (var card in results)
		{
			var cardResult = _searchResultScene.Instantiate<CardResult>();
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
