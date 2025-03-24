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
    private EventAggregator _events;
    private Match _match;
    private HexMap _map;
    private CardLibrary _cardDb;
    private IGameSettings _settings;

    public void Awake()
    {
        _events = Game.GetComponent<EventAggregator>();
        _match = Game.GetComponent<Match>();
        _map = Game.GetComponent<HexMap>();
        _cardDb = Game.GetComponent<CardLibrary>();
        _settings = Game.GetComponent<IGameSettings>();

        _events.Subscribe<BeginGameAction>(GameEvent.Prepare<BeginGameAction>(), OnPrepareBeginGame);
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

        // Set starting resources
        _match.LocalPlayer.Resources[ResourceType.Food] = _settings.StartingFoodCount;
        _match.LocalPlayer.Resources[ResourceType.Wood] = _settings.StartingWoodCount;
        _match.LocalPlayer.Resources[ResourceType.Gold] = _settings.StartingGoldCount;
        _match.LocalPlayer.Resources[ResourceType.Stone] = _settings.StartingStoneCount;

        _match.EnemyPlayer.Resources[ResourceType.Food] = _settings.StartingFoodCount;
        _match.EnemyPlayer.Resources[ResourceType.Wood] = _settings.StartingWoodCount;
        _match.EnemyPlayer.Resources[ResourceType.Gold] = _settings.StartingGoldCount;
        _match.EnemyPlayer.Resources[ResourceType.Stone] = _settings.StartingStoneCount;

        if(!_settings.DebugMode)
        {
            // Load player decks
            var deckInfo = new List<(int id, int amount)>
            {
                // TEMP: mocked deck info data using IDs from our DB, to test deck loading from disk
                // (2, 2), // 2 Knights
                (15, 1), // Agriculture
                // TODO: Implement and add a technology cards for each of these economic buildings
                (6, 1), // Lumber Camp
                (10, 1), // Mining Camp
                (13, 1), // Mill
            };

            var loadedPlayerDeck = _cardDb.LoadDeck(_match.LocalPlayer, deckInfo);
            var loadedEnemyDeck = _cardDb.LoadDeck(_match.EnemyPlayer, deckInfo);
            _match.LocalPlayer.Deck.AddRange(loadedPlayerDeck);
            _match.EnemyPlayer.Deck.AddRange(loadedEnemyDeck);
        }
    }

    private void OnPerformDiscardCards(DiscardCardsAction action)
    {
        foreach (var card in action.CardsToDiscard)
        {
            var player = card.Owner;
            // TODO: split into a separate reaction with a BanishCardsAction instead of putting this logic here
            if(card.CardData.Id == CardLibrary.VillagerId)
                player.MoveCard(card, Zone.Banished);
            else
                player.MoveCard(card, Zone.Discard);
        }
    }

    private void OnPerformShuffleDeck(ShuffleDeckAction action)
    {
        // NOTE: Do not shuffle decks in debug mode, so that we can guarantee card order during testing.
        if (!_settings.DebugMode)
        {
            var player = _match.Players[action.TargetPlayerId];
            player.Deck.Shuffle();
        }
    }
}
