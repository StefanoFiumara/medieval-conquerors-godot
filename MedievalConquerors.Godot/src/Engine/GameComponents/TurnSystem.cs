﻿using System.Linq;
using MedievalConquerors.Engine.Actions;
using MedievalConquerors.Engine.Actions.TurnActions;
using MedievalConquerors.Engine.Core;
using MedievalConquerors.Engine.Events;
using MedievalConquerors.UI;

namespace MedievalConquerors.Engine.GameComponents;

public class TurnSystem : GameComponent, IAwake
{
    private Match _match;
    private EventAggregator _events;
    private IGameSettings _settings;
    private CardLibrary _library;

    public void Awake()
    {
        _match = Game.GetComponent<Match>();
        _events = Game.GetComponent<EventAggregator>();
        _settings = Game.GetComponent<IGameSettings>();
        _library = Game.GetComponent<CardLibrary>();

        _events.Subscribe<ChangeTurnAction>(GameEvent.Perform<ChangeTurnAction>(), OnPerformChangeTurn);
        _events.Subscribe<BeginTurnAction>(GameEvent.Perform<BeginTurnAction>(), OnPerformBeginTurn);
        _events.Subscribe<BeginGameAction>(GameEvent.Perform<BeginGameAction>(), OnPerformBeginGame);
        _events.Subscribe(PlayerUiPanel.NextTurnClicked, OnClickNextTurn);
    }

    private void OnClickNextTurn()
    {
        if (!Game.IsIdle()) return;

        if (_match.CurrentPlayerId == _match.LocalPlayer.Id)
        {
            Game.Perform(new ChangeTurnAction(_match.EnemyPlayer.Id));
        }
    }

    private void OnPerformBeginGame(BeginGameAction action)
    {
        // Shuffle Decks
        Game.AddReaction(new ShuffleDeckAction(Match.LocalPlayerId));
        Game.AddReaction(new ShuffleDeckAction(Match.EnemyPlayerId));

        // Spawn Town Centers
        var townCenter1 = _library.LoadCard(CardLibrary.TownCenterId, _match.LocalPlayer);
        var townCenter2 = _library.LoadCard(CardLibrary.TownCenterId, _match.EnemyPlayer);
        Game.AddReaction(new PlayCardAction(townCenter1, _match.LocalPlayer.TownCenter.Position));
        Game.AddReaction(new PlayCardAction(townCenter2, _match.EnemyPlayer.TownCenter.Position));

        // Change the turn to the starting player
        Game.AddReaction(new ChangeTurnAction(action.StartingPlayerId));
    }

    private void OnPerformChangeTurn(ChangeTurnAction action)
    {
        if(_match.CurrentPlayer.Hand.Count > 0)
            Game.AddReaction(new DiscardCardsAction(_match.CurrentPlayer.Hand.ToList()));

        Game.AddReaction(new EndTurnAction(_match.CurrentPlayerId));
        Game.AddReaction(new BeginTurnAction(action.NextPlayerId));
        Game.AddReaction(new DrawCardsAction(action.NextPlayerId, _match.Players[action.NextPlayerId].TurnStartDrawCount));
        Game.AddReaction(new CollectResourcesAction(action.NextPlayerId));
    }

    private void OnPerformBeginTurn(BeginTurnAction action)
    {
        _match.CurrentPlayerId = action.PlayerId;
    }
}
