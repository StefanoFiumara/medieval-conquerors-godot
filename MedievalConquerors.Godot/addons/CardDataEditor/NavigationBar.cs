using System;
using Godot;
namespace MedievalConquerors.addons.CardDataEditor;

public enum EditorPanel
{
	Library, CardData 
}

[Tool]
public partial class NavigationBar : HBoxContainer
{
	public event Action LibraryPressed;
	public event Action EditorPressed;
	public event Action ReloadPressed;
	
	private Button _libraryBtn;
	private Button _editorBtn;
	private Button _reloadBtn;

	private EditorPanel _activePanel;

	public EditorPanel ActivePanel
	{
		get => _activePanel;
		set
		{
			_activePanel = value;
			_libraryBtn.Disabled = value == EditorPanel.Library;
			_editorBtn.Disabled = value == EditorPanel.CardData;
		}
	}
	
	public override void _Ready()
	{
		_libraryBtn = GetNode<Button>("%library_nav_btn");
		_editorBtn = GetNode<Button>("%editor_nav_btn");
		_reloadBtn = GetNode<Button>("%reload_plugin_btn");
		
		_editorBtn.Pressed += OnEditorBtnPressed;
		_reloadBtn.Pressed += OnReloadBtnPressed;
		_libraryBtn.Pressed += OnLibraryBtnPressed;

		ActivePanel = EditorPanel.Library;
	}

	private void OnReloadBtnPressed() => ReloadPressed?.Invoke();
	private void OnLibraryBtnPressed()
	{
		LibraryPressed?.Invoke();
	}

	private void OnEditorBtnPressed()
	{
		EditorPressed?.Invoke();
	}
}
