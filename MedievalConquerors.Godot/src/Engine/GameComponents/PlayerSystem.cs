using System.Collections.Generic;
using System.Linq;
using MedievalConquerors.Engine.Actions;
using MedievalConquerors.Engine.Core;
using MedievalConquerors.Engine.Data;
using MedievalConquerors.Engine.Events;
using MedievalConquerors.Extensions;

namespace MedievalConquerors.Engine.GameComponents;

public class PlayerSystem : GameComponent, IAwake
{
    private IEventAggregator _events;
    private Match _match;
    private HexMap _map;
    private CardLibrary _cardDb;

    public void Awake()
    {
        _events = Game.GetComponent<EventAggregator>();
        _match = Game.GetComponent<Match>();
        _map = Game.GetComponent<HexMap>();
        _cardDb = Game.GetComponent<CardLibrary>();
        
        _events.Subscribe<BeginGameAction>(GameEvent.Prepare<BeginGameAction>(), OnPrepareBeginGame);
        _events.Subscribe<PlayCardAction>(GameEvent.Perform<PlayCardAction>(), OnPerformPlayCard);
        _events.Subscribe<DiscardCardsAction>(GameEvent.Perform<DiscardCardsAction>(), OnPerformDiscardCards);
        _events.Subscribe<ShuffleDeckAction>(GameEvent.Perform<ShuffleDeckAction>(), OnPerformShuffleDeck);
    }

    private void OnPrepareBeginGame(BeginGameAction action)
    {
        // Set town centers based on map
        _match.LocalPlayer.TownCenter = _map.SearchTiles(t => t.Terrain == TileTerrain.StartingTownCenterBlue).Single();
        _match.EnemyPlayer.TownCenter = _map.SearchTiles(t => t.Terrain == TileTerrain.StartingTownCenterRed).Single();
         
        _map.SetTile(_match.LocalPlayer.TownCenter.Position, TileTerrain.Grass);
        _map.SetTile(_match.EnemyPlayer.TownCenter.Position, TileTerrain.Grass);
        
        // TODO: Load preset decks when debug mode is enabled, so that unit tests can use custom test decks for the player
        // TEMP: mocked deck info data using IDs from our DB, to test deck loading from disk
        var deckInfo = new List<(int id, int amount)>
        {
            (11, 2), // 2 Villagers
            (2, 2), // 2 Knights
            (6, 1), // Lumber Camp
            (10, 1), // Mining Camp
            (13, 1), // Mill
        };
        var loadedPlayerDeck = _cardDb.LoadDeck(_match.LocalPlayer, deckInfo);
        var loadedEnemyDeck = _cardDb.LoadDeck(_match.EnemyPlayer, deckInfo);
        _match.LocalPlayer.Deck.AddRange(loadedPlayerDeck);
        _match.EnemyPlayer.Deck.AddRange(loadedEnemyDeck);
    }

    private void OnPerformDiscardCards(DiscardCardsAction action)
    {
        action.Target.MoveCards(action.CardsToDiscard, Zone.Discard);
    }

    private void OnPerformPlayCard(PlayCardAction action)
    {
        action.SourcePlayer.MoveCard(action.CardToPlay, Zone.Map);
    }

    private void OnPerformShuffleDeck(ShuffleDeckAction action)
    {
        var player = _match.Players[action.TargetPlayerId];
        player.Deck.Shuffle();
    }
}