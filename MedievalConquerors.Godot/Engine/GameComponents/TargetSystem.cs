using System.Collections.Generic;
using System.Linq;
using Godot;
using MedievalConquerors.Engine.Core;
using MedievalConquerors.Engine.Data;
using MedievalConquerors.Engine.Data.Attributes;
using MedievalConquerors.Extensions;

namespace MedievalConquerors.Engine.GameComponents;

public class TargetSystem : GameComponent, IAwake
{
    private HexMap _gameMap;

    public void Awake()
    {
        _gameMap = Game.GetComponent<HexMap>();
    }

    public List<Vector2I> GetTargetCandidates(Card source)
    {
        var spawnPointAttribute = source.GetAttribute<SpawnPointAttribute>();
        
        if(spawnPointAttribute == null)
            return new List<Vector2I>();

        if (spawnPointAttribute.SpawnTags == Tags.TownCenter)
        {
            // NOTE: Ignore spawn range property and instead use player's range of influence
            return _gameMap.GetReachable(source.Owner.TownCenter.Position, source.Owner.InfluenceRange).ToList();
        }
        
        var targetCandidates = new List<Vector2I>();
        // TODO: We may want to allow the card type to be configurable in SpawnPointAttribute
        //       if we want to spawn units on other units or non-buildings 
        var buildings = source.Owner.Map.Where(c => c.CardData.CardType == CardType.Building);

        foreach (var building in buildings)
        {
            if (building.CardData.Tags.HasFlag(spawnPointAttribute.SpawnTags))
            {
                var surroundingNeighbors = _gameMap.GetReachable(building.MapPosition, spawnPointAttribute.SpawnRange);
                targetCandidates.AddRange(surroundingNeighbors);
            }
        }

        return targetCandidates;
    }
}