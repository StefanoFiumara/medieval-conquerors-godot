﻿using System.Collections.Generic;
using System.Linq;
using Godot;
using MedievalConquerors.Engine.Actions;
using MedievalConquerors.Engine.Core;
using MedievalConquerors.Engine.Data;
using MedievalConquerors.Engine.Data.Attributes;
using MedievalConquerors.Engine.Events;
using MedievalConquerors.Extensions;

namespace MedievalConquerors.Engine.GameComponents;

public class TargetSystem : GameComponent, IAwake
{
    private HexMap _map;
    private EventAggregator _events;

    public void Awake()
    {
        _events = Game.GetComponent<EventAggregator>();
        _map = Game.GetComponent<HexMap>();
        _events.Subscribe<PlayCardAction, ActionValidatorResult>(GameEvent.Validate<PlayCardAction>(), OnValidatePlayCard);
    }

    private void OnValidatePlayCard(PlayCardAction action, ActionValidatorResult validator)
    {
        var tile = _map.GetTile(action.TargetTile);
        var validTiles = GetTargetCandidates(action.CardToPlay);

        if (!validTiles.Contains(tile.Position))
            validator.Invalidate("Invalid target tile for card.");
    }
    
    public List<Vector2I> GetTargetCandidates(Card source)
    {
        var player = source.Owner;
        var spawnPointAttribute = source.GetAttribute<SpawnPointAttribute>();
        var buildings = player.Map.Where(c => c.CardData.CardType == CardType.Building);
        
        if(spawnPointAttribute == null)
            return new List<Vector2I>();

        if (spawnPointAttribute.SpawnTags == Tags.TownCenter)
            return _map.GetReachable(player.TownCenter.Position, player.InfluenceRange).ToList();

        if (spawnPointAttribute.SpawnRange == 0)
            return buildings.Select(b => b.MapPosition).ToList();
        
        var targetCandidates = new List<Vector2I>();
        foreach (var building in buildings)
        {
            if (building.CardData.Tags.HasFlag(spawnPointAttribute.SpawnTags))
            {
                var surroundingNeighbors = _map.GetReachable(building.MapPosition, spawnPointAttribute.SpawnRange);
                targetCandidates.AddRange(surroundingNeighbors);
            }
        }

        return targetCandidates;
    }
}