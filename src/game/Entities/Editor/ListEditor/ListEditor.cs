using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using MedievalConquerors.Entities.Editor.ValueEditors;
using MedievalConquerors.Extensions;

namespace MedievalConquerors.Entities.Editor;

public partial class ListEditor<T> : PanelContainer, IListEditor<T>
	where T : class
{
	private static readonly PackedScene _objectEditor = GD.Load<PackedScene>("uid://bxlv4w3wwtsro");

	[Export] private string _title;
	[Export] private bool _allowDelete;

	private Label _titleLabel;
	private Button _removeButton;

	private TypeOptions<T> _typeSelector;
	private VBoxContainer _editorsContainer;
	private Button _addButton;

	private IEnumerable<IObjectEditor> Editors =>
		_editorsContainer.GetChildren().OfType<IObjectEditor>();

	public override void _Ready()
	{
		_titleLabel = GetNode<Label>("%title_label");
		_removeButton = GetNode<Button>("%close_button");
		_addButton = GetNode<Button>("%add_item_btn");
		_editorsContainer = GetNode<VBoxContainer>("%editors_container");

		_titleLabel.Text = _title ?? string.Empty;
		_removeButton.Visible = _allowDelete;

		_typeSelector = CreateTypeSelector();
		if (_typeSelector != null)
		{
			var controls = GetNode<HBoxContainer>("%editor_controls");
			controls.AddChild(_typeSelector);
			controls.MoveChild(_typeSelector, 2);
			_typeSelector.Connect(OptionButton.SignalName.ItemSelected, Callable.From<long>(OnNewTypeSelected));
		}

		_addButton.Connect(BaseButton.SignalName.Pressed, Callable.From(CreateNewEditor));
	}

	private static TypeOptions<T> CreateTypeSelector()
	{
		var optionsType = AppDomain.CurrentDomain.GetAssemblies()
			.SelectMany(a => a.GetTypes())
			.SingleOrDefault(t => t.IsClass && !t.IsAbstract && t.IsAssignableTo(typeof(TypeOptions<T>)));

		if (optionsType != null)
		{
			return (TypeOptions<T>) Activator.CreateInstance(optionsType);
		}

		return null;
	}

	public void Load(string title, List<T> source, bool allowDelete = false)
	{
		Reset();
		_titleLabel.Text = _title ?? title ?? string.Empty;
		_removeButton.Visible = _allowDelete || allowDelete;

		foreach (var item in source.OrderBy(t => t.GetType().Name))
			AddNewEditor(item);
	}

	public void Reset()
	{
		foreach (var editor in Editors)
			editor.GetControl().QueueFree();
	}

	public void Enable()
	{
		_typeSelector?.Disabled = false;
		_addButton.Disabled = _typeSelector?.Selected == 0;

		foreach (var editor in Editors)
			editor.Enable();
	}

	public void Disable()
	{
		_typeSelector?.Disabled = true;
		_addButton.Disabled = true;

		foreach (var editor in Editors)
			editor.Disable();
	}

	public List<T> Create() => Editors.Select(e => e.Create()).Cast<T>().ToList();
	public Control GetControl() => this;

	private void CreateNewEditor()
	{
		var instance = _typeSelector == null
				? Activator.CreateInstance<T>()
				: (T) Activator.CreateInstance(_typeSelector.SelectedType);

		AddNewEditor(instance);
		ClearSelector();
	}

	private void AddNewEditor(T source)
	{
		var editor = _objectEditor.Instantiate<IObjectEditor>();
		editor.Load(title: $"{source.GetType().Name.Replace("Attribute", string.Empty).PrettyPrint()}",
			source: source,
			allowDelete: true);

		_editorsContainer.AddChild(editor.GetControl());
	}

	private void ClearSelector()
	{
		if (_typeSelector == null) return;

		_typeSelector.Select(0);
		OnNewTypeSelected(0);
	}

	private void OnNewTypeSelected(long itemIndex)
	{
		if (_typeSelector == null) return;

		var selectedText = _typeSelector.GetItemText((int)itemIndex);
		_addButton.Disabled = selectedText == "None" || Editors.Any(e => e.ObjectType.Name == selectedText);
	}
}
