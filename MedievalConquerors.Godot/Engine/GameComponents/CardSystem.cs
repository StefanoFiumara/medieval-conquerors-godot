using System.Collections.Generic;
using System.Linq;
using MedievalConquerors.Engine.Actions;
using MedievalConquerors.Engine.Core;
using MedievalConquerors.Engine.Data;
using MedievalConquerors.Extensions;

namespace MedievalConquerors.Engine.GameComponents;

// TODO: Unit tests for this component
public class CardSystem : GameComponent, IAwake
{
    private readonly List<Card> _playable = new();

    private Match _match;
    private IMap _map;

    public void Awake()
    {
        _match = Game.GetComponent<Match>();
        _map = Game.GetComponent<IMap>();
    }

    public bool IsPlayable(Card card) => _playable.Contains(card);

    public void Refresh()
    {
        _playable.Clear();
        
        foreach (var card in _match.CurrentPlayer.Hand)
        {
            // TODO: more robust check for available tiles for this card, query attributes?
            var randomTargetTile = _map.SearchTiles(t => t.Terrain == TileTerrain.Grass).GetRandom();
            var playAction = new PlayCardAction(card, randomTargetTile.Position);
            if (playAction.Validate(Game).IsValid)
            {
                _playable.Add(card);
            }
        }
    }
}