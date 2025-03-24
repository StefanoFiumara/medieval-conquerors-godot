using Godot;
using MedievalConquerors.Engine.Data;
using MedievalConquerors.Engine.Events;
using MedievalConquerors.Engine.GameComponents;

namespace MedievalConquerors.UI;

public partial class PlayerUiPanel : MarginContainer
{
	public const string NextTurnClicked = "PlayerUiPanel.NextTurnButton.Clicked";

	[Export] private GameController _gameController;

	private Button _endTurnButton;
	private Label _foodLabel;
	private Label _woodLabel;
	private Label _goldLabel;
	private Label _stoneLabel;
	private Label _ageLabel;
	private Label _deckLabel;

	private EventAggregator _events;
	private Match _match;

	public override void _Ready()
	{
		_endTurnButton = GetNode<Button>("%end_turn_button");
		_foodLabel = GetNode<Label>("%food_label");
		_woodLabel = GetNode<Label>("%wood_label");
		_goldLabel = GetNode<Label>("%gold_label");
		_stoneLabel = GetNode<Label>("%stone_label");
		_ageLabel = GetNode<Label>("%age_label");
		_deckLabel = GetNode<Label>("%deck_label");

		_events = _gameController.Game.GetComponent<EventAggregator>();
		_match = _gameController.Game.GetComponent<Match>();

		_endTurnButton.ButtonUp += () => _events.Publish(NextTurnClicked);
		_events.Subscribe(ActionSystem.BeginActionEvent, OnBeginSequence);
		_events.Subscribe(ActionSystem.CompleteActionEvent, OnActionsComplete);
	}
	private void OnBeginSequence() => _endTurnButton.Disabled = true;

	private void OnActionsComplete()
	{
		if(_match.CurrentPlayerId == _match.LocalPlayer.Id)
			_endTurnButton.Disabled = false;
	}

	public override void _Process(double delta)
	{
		// TODO: Instead of setting this in _Process, subscribe to game actions that update these values and make updates there.
		_foodLabel.Text = _match.LocalPlayer.Resources.Food.ToString();
		_woodLabel.Text = _match.LocalPlayer.Resources.Wood.ToString();
		_goldLabel.Text = _match.LocalPlayer.Resources.Gold.ToString();
		_stoneLabel.Text = _match.LocalPlayer.Resources.Stone.ToString();
		_ageLabel.Text = _match.LocalPlayer.Age.PrettyPrint();
		_deckLabel.Text = _match.LocalPlayer.Deck.Count.ToString();
	}

	public override void _ExitTree()
	{
		_events.Unsubscribe(ActionSystem.BeginActionEvent, OnBeginSequence);
		_events.Unsubscribe(ActionSystem.CompleteActionEvent, OnActionsComplete);
	}
}
