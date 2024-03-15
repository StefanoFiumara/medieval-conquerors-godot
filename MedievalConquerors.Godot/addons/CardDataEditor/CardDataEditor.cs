using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Godot;
using MedievalConquerors.Addons.CardDataEditor.Controls;
using MedievalConquerors.Addons.CardDataEditor.UIStates;
using MedievalConquerors.Engine.Data;
using MedievalConquerors.Engine.StateManagement;
using MedievalConquerors.Extensions;

namespace MedievalConquerors.Addons.CardDataEditor;

[Tool]
public partial class CardDataEditor : ScrollContainer
{
	private CardData _loadedData;
	private StateMachine _stateMachine;
	private bool _isDirty;
	
	// UI Components
	public RichTextLabel PanelTitle { get; private set; }

	public Button NewButton { get; private set; }
	public Button SaveButton { get; private set; }
	public Button AddAttributeButton { get; private set; }
	
	public LineEdit CardTitle { get; private set; }
	public TextEdit Description { get; private set; }
	public CardTypeOptions CardTypeOptions { get; private set; }
	public TagOptions TagOptions { get; private set; }
	public AttributeOptions AttributeSelector { get; private set; }
	
	public VBoxContainer CustomAttributeList { get; private set; }

	public bool IsDirty
	{
		get => _isDirty;
		set
		{
			_isDirty = value;
			SaveButton.Disabled = !_isDirty;
		}
	}

	public CardData LoadedData
	{
		get => _loadedData;
		private set
		{
			_loadedData = value;
			if (value != null)
			{
				Reset();
				CardTitle.Text = _loadedData.Title;
				Description.Text = _loadedData.Description;
				CardTypeOptions.SelectedCardType = _loadedData.CardType;
				TagOptions.SelectedTags = _loadedData.Tags;

				foreach (var attr in value.Attributes)
				{
					var editor = CreateAttributeEditor(attr);
					CustomAttributeList.AddChild(editor);
				}
			}
			
			SaveButton.Disabled = _loadedData == null;
		}
	}
	
	public override void _Ready()
	{
		PanelTitle = GetNode<RichTextLabel>("%currently_editing");
		SaveButton = GetNode<Button>("%save_btn");
		NewButton = GetNode<Button>("%new_btn");
		CardTitle = GetNode<LineEdit>("%title_edit");
		Description = GetNode<TextEdit>("%desc_edit");
		CardTypeOptions = GetNode<CardTypeOptions>("%card_type_selector");
		TagOptions = GetNode<TagOptions>("%tags_grid");
		AttributeSelector = GetNode<AttributeOptions>("%attr_selector");
		AddAttributeButton = GetNode<Button>("%add_attr_btn");
		CustomAttributeList = GetNode<VBoxContainer>("%custom_attribute_list");

		LoadedData = null;
		_stateMachine = new StateMachine(new NoDataState(this));

		CardTitle.TextChanged += _ => IsDirty = CardTitle.Text != LoadedData?.Title;
		Description.LinesEditedFrom += (_, _) => IsDirty = Description.Text != LoadedData?.Description;
		CardTypeOptions.ItemSelected += _ => IsDirty = CardTypeOptions.SelectedCardType != LoadedData?.CardType;
		TagOptions.TagsChanged += () => IsDirty = TagOptions.SelectedTags != LoadedData?.Tags;
		AttributeSelector.ItemSelected += OnAttributeSelectorItemSelected;
		AddAttributeButton.Pressed += CreateNewAttribute;
		SaveButton.Pressed += SaveCardResource;
		NewButton.Pressed += CreateNewCard;
	}

	private void OnAttributeSelectorItemSelected(long i)
	{
		var selectedText = AttributeSelector.GetItemText((int)i);
		AddAttributeButton.Disabled = selectedText == "None" || LoadedData?.Attributes.Any(a => a.GetType().Name == selectedText) == true;
	}

	private void Reset()
	{
		CardTitle.Text = string.Empty;
		Description.Text = string.Empty;
		CardTypeOptions.SelectedCardType = CardType.None;
		TagOptions.SelectedTags = Tags.None;

		var attributeControls = CustomAttributeList.GetChildren();
		foreach (var control in attributeControls)
		{
			control.QueueFree();
		}
		
		AttributeSelector.Select(0);
		OnAttributeSelectorItemSelected(0);
	}
	
	private void CreateNewCard()
	{
		LoadedData = new CardData();
		_stateMachine.ChangeState(new CreatingNewCardState(this));
		Reset();
		GD.Print("New Card Data Resource Created");
	}

	private void CreateNewAttribute()
	{
		var attr = AttributeSelector.CreateSelected();
		var editor = CreateAttributeEditor(attr);
		
		CustomAttributeList.AddChild(editor);
		LoadedData.Attributes.Add(attr);
		
		AttributeSelector.Select(0);
		OnAttributeSelectorItemSelected(0);
		
		IsDirty = true;
	}
	
	private Control CreateAttributeEditor(ICardAttribute attr)
	{
		// TODO: Convert this to a scene that just dynamically creates the grid container
		var vbox = new VBoxContainer();
		var title = new Label();
		title.Text = attr.GetType().Name;
		title.Modulate = Colors.Goldenrod;
		
		var props = attr.GetType().GetProperties()
			.Where(p => p.GetSetMethod() != null)
			.ToList();
		
		var gridContainer = new MarginContainer();
		gridContainer.AddThemeConstantOverride("margin_left", 100);
		var grid = CreatePropsGrid(attr, props);
		gridContainer.AddChild(grid);
		
		vbox.AddChild(new HSeparator());
		var hbox = new HBoxContainer();
		hbox.AddChild(title);
		var removeButton = new Button();
		removeButton.Text = "X";
		removeButton.AddThemeColorOverride("font_color", Colors.Red);
		removeButton.AddThemeColorOverride("font_hover_color", Colors.Red);
		hbox.AddChild(removeButton);
		vbox.AddChild(hbox);
		vbox.AddChild(gridContainer);
		
		removeButton.Pressed += () =>
		{
			vbox.QueueFree();
			LoadedData.Attributes.Remove(attr);
			IsDirty = true;
		};
		
		return vbox;
	}

	private GridContainer CreatePropsGrid(ICardAttribute attr, List<PropertyInfo> props)
	{
		var propGrid = new GridContainer
		{
			Columns = 2
		};
		foreach (var prop in props)
		{
			var label = new Label();
			label.Text = $"{prop.Name}: ";
			label.HorizontalAlignment = HorizontalAlignment.Right;
			
			var editor = CreatePropertyEditor(attr, prop);
			if (editor != null)
			{
				propGrid.AddChild(label);
				propGrid.AddChild(editor);
			}
		}

		return propGrid;
	}

	private Control CreatePropertyEditor(ICardAttribute attr, PropertyInfo prop)
	{
		if (prop.PropertyType == typeof(string))
		{
			var editor = new LineEdit();
			editor.TextChanged += txt =>
			{
				prop.SetValue(attr, txt);
				IsDirty = true;
			};
			return editor;
		}

		if (prop.PropertyType == typeof(int))
		{
			var editor = new SpinBox();
			editor.ValueChanged += v =>
			{
				prop.SetValue(attr, (int)v);
				IsDirty = true;
			};
			return editor;
		}

		return null;
	}

	private void UpdateLoadedResourceFromEditor()
	{
		LoadedData.Title = CardTitle.Text.Trim();
		LoadedData.Description = Description.Text.Trim();
		LoadedData.CardType = CardTypeOptions.SelectedCardType;
		LoadedData.Tags = TagOptions.SelectedTags;
		//NOTE: Attributes are automatically updated
	}

	private void SaveCardResource()
	{
		if (LoadedData == null) return;
		
		UpdateLoadedResourceFromEditor();

		try
		{
			using var db = new CardDatabase();
			LoadedData.Id = db.SaveCardData(LoadedData);
			GD.PrintRich("Successfully save Card Data".Green());
			
			_stateMachine.ChangeState(new EditingExistingCardState(this));
		}
		catch(Exception e)
		{
			GD.PrintErr($"Failed to save Card Data");
			GD.PrintErr(e.ToString());
		}
	}
}
