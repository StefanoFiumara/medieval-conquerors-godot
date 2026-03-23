using System.Collections.Generic;
using System.Linq;
using Godot;
using MedievalConquerors.Engine.Core;
using MedievalConquerors.Engine.Data;

namespace MedievalConquerors.Engine.Attributes.TargetSelectors;

public record AdjacentTerrainSelector : TargetSelector
{
    public TileTerrain Terrain { get; init; }

    public override List<Vector2I> SelectTargets(IGame game, Card card)
    {
        var map = game.GetComponent<HexMap>();
        var player = card.Owner;

        return map.GetReachable(player.TownCenter.Position, player.InfluenceRange)
            .Where(t => map.GetNeighbors(t).Any(n => n.Terrain == Terrain))
            .ToList();
    }
}
