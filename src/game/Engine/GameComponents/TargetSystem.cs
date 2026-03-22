using System.Collections.Generic;
using System.Linq;
using Godot;
using MedievalConquerors.Engine.Actions;
using MedievalConquerors.Engine.Attributes;
using MedievalConquerors.Engine.Core;
using MedievalConquerors.Engine.Data;

using MedievalConquerors.Engine.Events;

namespace MedievalConquerors.Engine.GameComponents;

public class TargetSystem : GameComponent, IAwake
{
    private HexMap _map;
    private EventAggregator _events;
    private GarrisonSystem _garrisonSystem;

    public void Awake()
    {
        _events = Game.GetComponent<EventAggregator>();
        _map = Game.GetComponent<HexMap>();
        _garrisonSystem = Game.GetComponent<GarrisonSystem>();

        _events.Subscribe<PlayCardAction, ActionValidatorResult>(GameEvent.Validate<PlayCardAction>(), OnValidatePlayCard);
    }

    private void OnValidatePlayCard(PlayCardAction action, ActionValidatorResult validator)
    {
        // TODO: support cards without target tile
        var tile = _map.GetTile(action.TargetTile);
        var validTiles = GetTargetCandidates(action.CardToPlay);

        if (!validTiles.Contains(tile.Position))
            validator.Invalidate("Invalid target tile for card.");
    }

    public List<Vector2I> GetTargetCandidates(Card card)
    {
        return card.HasAttribute<TargetSelectorAttribute>(out var selector)
            ? selector.SelectTargets(Game, card)
            : [];
    }
}
