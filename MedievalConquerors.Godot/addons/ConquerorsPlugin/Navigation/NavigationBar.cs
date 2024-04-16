using System;
using Godot;

namespace MedievalConquerors.ConquerorsPlugin.Navigation;

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
	
	[Export] private Button _libraryBtn;
	[Export] private Button _editorBtn;
	[Export] private Button _reloadBtn;

	private EditorPanel _activePanel;
	public EditorPanel ActivePanel
	{
		get => _activePanel;
		set
		{
			_activePanel = value;
			if(_libraryBtn != null) _libraryBtn.Disabled = value == EditorPanel.Library;
			if(_editorBtn != null) _editorBtn.Disabled = value == EditorPanel.CardData;
		}
	}
	
	public override void _EnterTree()
	{
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

	public override void _ExitTree()
	{
		_editorBtn.Pressed -= OnEditorBtnPressed;
		_reloadBtn.Pressed -= OnReloadBtnPressed;
		_libraryBtn.Pressed -= OnLibraryBtnPressed;
	}
}
