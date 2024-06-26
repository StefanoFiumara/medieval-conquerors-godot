using System.Linq;
using MedievalConquerors.Engine.Actions;
using MedievalConquerors.Engine.Core;
using MedievalConquerors.Engine.Data;
using MedievalConquerors.Engine.Events;
using MedievalConquerors.Extensions;
using MedievalConquerors.Views.Maps;

namespace MedievalConquerors.Engine.GameComponents;

public class PlayerSystem : GameComponent, IAwake
{
    private IEventAggregator _events;
    private Match _match;
    private HexMap _map;

    public void Awake()
    {
        _events = Game.GetComponent<EventAggregator>();
        _match = Game.GetComponent<Match>();
        _map = Game.GetComponent<HexMap>();
        
        _events.Subscribe<BeginGameAction>(GameEvent.Prepare<BeginGameAction>(), OnPrepareBeginGame);
        _events.Subscribe<PlayCardAction>(GameEvent.Perform<PlayCardAction>(), OnPerformPlayCard);
        _events.Subscribe<DiscardCardsAction>(GameEvent.Perform<DiscardCardsAction>(), OnPerformDiscardCards);
        _events.Subscribe<ShuffleDeckAction>(GameEvent.Perform<ShuffleDeckAction>(), OnPerformShuffleDeck);
    }

    private void OnPrepareBeginGame(BeginGameAction action)
    {
        // Set town centers based on map
        var townCenters = _map.SearchTiles(t => t.Terrain == TileTerrain.TownCenter);
        // TODO: Find where the TCs are on the map and assign them to the player
        // TODO: Do we want a building card token here ?
        _match.LocalPlayer.TownCenter = townCenters.First();
        _match.EnemyPlayer.TownCenter = townCenters.Last();

        // TODO: this should updated dynamically as Player's Influence Range changes, perhaps in MapView when responding to some actions.
        // var tilesInfluencedLocal = _map.GetReachable(match.LocalPlayer.TownCenter.Position, match.LocalPlayer.InfluenceRange);
        // _mapView.HighlightTiles(tilesInfluencedLocal, HighlightLayer.BlueTeam);
        //
        // var tilesInfluencedEnemy = _map.GetReachable(match.EnemyPlayer.TownCenter.Position, match.EnemyPlayer.InfluenceRange);
        // _mapView.HighlightTiles(tilesInfluencedEnemy, HighlightLayer.RedTeam);
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