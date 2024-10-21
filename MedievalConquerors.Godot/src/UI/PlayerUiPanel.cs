using Godot;
using MedievalConquerors.Engine.Actions;
using MedievalConquerors.Engine.Data;
using MedievalConquerors.Engine.Events;
using MedievalConquerors.Engine.GameComponents;

namespace MedievalConquerors.UI;

public partial class PlayerUiPanel : MarginContainer
{
	[Export] private GameController _gameController;

	private Button _endTurnButton;
	private Label _storageLabel;
	private Label _foodLabel;
	private Label _woodLabel;
	private Label _goldLabel;
	private Label _stoneLabel;
	private Label _ageLabel;
	private Label _deckLabel;
	
	private Match _match;
	
	public override void _Ready()
	{
		_endTurnButton = GetNode<Button>("%end_turn_button");
		_storageLabel = GetNode<Label>("%storage_label");
		_foodLabel = GetNode<Label>("%food_label");
		_woodLabel = GetNode<Label>("%wood_label");
		_goldLabel = GetNode<Label>("%gold_label");
		_stoneLabel = GetNode<Label>("%stone_label");
		_ageLabel = GetNode<Label>("%age_label");
		_deckLabel = GetNode<Label>("%deck_label");
		
		_match = _gameController.Game.GetComponent<Match>();
		
		_endTurnButton.ButtonUp += OnClickNextTurn;
	}

	private void OnClickNextTurn()
	{
		if (_match.CurrentPlayerId == _match.LocalPlayer.Id)
		{
			_gameController.Game.Perform(new ChangeTurnAction(_match.EnemyPlayer.Id));
		}
	}

	public override void _Process(double delta)
	{
		_storageLabel.Text = $"{_match.LocalPlayer.Resources.TotalResources}/{_match.LocalPlayer.Resources.StorageLimit}";
		_foodLabel.Text = _match.LocalPlayer.Resources.Food.ToString();
		_woodLabel.Text = _match.LocalPlayer.Resources.Wood.ToString();
		_goldLabel.Text = _match.LocalPlayer.Resources.Gold.ToString();
		_stoneLabel.Text = _match.LocalPlayer.Resources.Stone.ToString();
		_ageLabel.Text = _match.LocalPlayer.Age.PrettyPrint();
		_deckLabel.Text = _match.LocalPlayer.Deck.Count.ToString();
	}

	public override void _ExitTree()
	{
		_endTurnButton.Pressed -= OnClickNextTurn;
	}
}
