using System.Collections;
using Godot;
using MedievalConquerors.Engine.Actions;
using MedievalConquerors.Engine.Core;
using MedievalConquerors.Engine.Data;
using MedievalConquerors.Engine.Events;

namespace MedievalConquerors.Entities.Hand;

public partial class DeckCycleView : CpuParticles2D, IGameComponent
{
	private HandView _hand;
	private EventAggregator _events;

	public IGame Game { get; set; }

	public override void _EnterTree()
	{
		_hand = GetParent<HandView>();
		_hand.Game.AddComponent(this);
		_events = Game.GetComponent<EventAggregator>();
	}

	public override void _Ready()
	{
		_events.Subscribe<CycleDeckAction>(GameEvent.Prepare<CycleDeckAction>(), OnPrepareCycleDeck);
	}

	private void OnPrepareCycleDeck(CycleDeckAction action) => action.PerformPhase.Viewer = DeckCycleAnimation;

	private IEnumerator DeckCycleAnimation(IGame game, GameAction action)
	{
		var deckCycleAction = (CycleDeckAction) action;
		var match = Game.GetComponent<Match>();
		var player = match.Players[deckCycleAction.TargetPlayerId];

		if (player != match.LocalPlayer) yield break;

		Amount = Mathf.Max(player.Discard.Count, 5);
		Restart();

		while (Emitting) yield return null;

		// var additionalWait = Lifetime * 0.3f;
		// double elapsed = 0;
		// while(elapsed < additionalWait)
		// {
		// 	elapsed += GetProcessDeltaTime();
		// 	yield return null;
		// }
	}
}
