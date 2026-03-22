using System.Collections.Generic;
using System.Linq;
using Godot;
using MedievalConquerors.Engine.Core;
using MedievalConquerors.Engine.Data;

namespace MedievalConquerors.Engine.Attributes.TargetSelectors;

public record TileWithinInfluenceSelector : TargetSelector
{
    protected override List<Vector2I> SelectTargets(IGame game, Card card)
    {
        var player = card.Owner;
        var map = game.GetComponent<HexMap>();

        return map.GetReachable(player.TownCenter.Position, player.InfluenceRange).ToList();
    }
}