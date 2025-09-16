using System.Collections;
using System.Collections.Generic;
using MedievalConquerors.Engine.Actions;
using MedievalConquerors.Engine.Core;
using MedievalConquerors.Engine.Events;
using MedievalConquerors.Engine.Logging;

namespace MedievalConquerors.Engine.GameComponents;

public class ActionSystem : GameComponent, IAwake, IUpdate
{
	public const string BEGIN_ACTION_EVENT = "ActionSystem.beginActionEvent";
	public const string END_ACTION_EVENT = "ActionSystem.endActionEvent";
	public const string COMPLETE_ACTION_EVENT = "ActionSystem.completeActionEvent";

	public bool IsActive => _rootSequence != null;

	private GameAction _rootAction;
	private IEnumerator _rootSequence;
	private List<GameAction> _openReactions;
	// private VictorySystem _victorySystem;
	private EventAggregator _events;
	private ILogger _logger;

	public void Awake()
	{
		_events = Game.GetComponent<EventAggregator>();
		_logger = Game.GetComponent<ILogger>();
		// _victorySystem = Game.GetComponent<VictorySystem>();
	}

	public void Perform(GameAction action)
	{
		if (IsActive)
		{
			_logger.Error("Error: Attempted to perform GameAction while sequence in progress.");
			return;
		}

		_rootAction = action;
		_rootSequence = Sequence(action);
		_logger.Info($"Perform <{action}>");
	}

	public void Update()
	{
		if (_rootSequence == null) return;

		if (_rootSequence.MoveNext() == false)
		{
			_rootAction = null;
			_rootSequence = null;
			_openReactions = null;
			_events.Publish(COMPLETE_ACTION_EVENT);
		}
	}

	public void AddReaction(GameAction action)
	{
		if (_openReactions == null)
		{
			_logger.Error($"Attempted to add a reaction ({action.GetType().Name}) at the wrong time.");
			return;
		}

		_openReactions.Add(action);
	}

	private IEnumerator Sequence(GameAction action)
	{
		_events.Publish(BEGIN_ACTION_EVENT, action);

		var validationResult = action.Validate(Game);
		if (validationResult.IsValid == false)
		{
			foreach (var reason in validationResult.ValidationErrors)
			{
				_logger.Info($"\t* Action Invalidated: {reason}");
			}
			action.Cancel();
		}

		// TODO: Re-implement
		// if (_victorySystem.CheckGameOver())
		// {
		//     GD.Print("Game Over was detected, all pending game actions will be canceled.");
		//     action.Cancel();
		// }

		var phase = MainPhase(action.PreparePhase);
		while (phase.MoveNext())
			yield return null;

		phase = MainPhase(action.PerformPhase);
		while (phase.MoveNext())
			yield return null;

		phase = MainPhase(action.CancelPhase);
		while (phase.MoveNext())
			yield return null;

		_events.Publish(END_ACTION_EVENT, action);
	}

	private IEnumerator MainPhase (ActionPhase phase)
	{
		if (phase.Owner.IsCanceled ^ phase == phase.Owner.CancelPhase)
			yield break;

		_openReactions = [];

		var flow = phase.Flow (Game);
		while (flow.MoveNext())
			yield return null;

		flow = ReactPhase (_openReactions);
		while (flow.MoveNext())
			yield return null;
	}

	private IEnumerator ReactPhase(List<GameAction> reactions)
	{
		reactions.Sort(SortActions);
		foreach (var reaction in reactions)
		{
			_logger.Info($"\t* Reaction: <{reaction}>");

			var subFlow = Sequence(reaction);
			while (subFlow.MoveNext())
				yield return null;
		}
	}

	private static int SortActions(GameAction x, GameAction y) =>
		x.Priority != y.Priority
			? y.Priority.CompareTo(x.Priority)
			: x.OrderOfPlay.CompareTo(y.OrderOfPlay);
}

public static class ActionSystemExtensions
{
	public static void Perform(this IGame game, GameAction action)
	{
		var actionSystem = game.GetComponent<ActionSystem>();
		actionSystem.Perform(action);
	}

	public static void AddReaction(this IGame game, GameAction action)
	{
		var actionSystem = game.GetComponent<ActionSystem>();
		actionSystem.AddReaction(action);
	}
}
