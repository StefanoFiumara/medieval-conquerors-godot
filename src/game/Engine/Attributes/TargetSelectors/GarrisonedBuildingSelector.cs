using System.Collections.Generic;
using System.Linq;
using Godot;
using MedievalConquerors.Engine.Core;
using MedievalConquerors.Engine.Data;
using MedievalConquerors.Engine.GameComponents;

namespace MedievalConquerors.Engine.Attributes.TargetSelectors;

public record GarrisonedBuildingSelector : TargetSelector
{
    public bool UnspentOnly { get; init; }

    public override List<Vector2I> SelectTargets(IGame game, Card card)
    {
        var player = card.Owner;
        var garrisonSystem = game.GetComponent<GarrisonSystem>();
        var resourceGatheringSystem = game.GetComponent<ResourceGatheringSystem>();

        var tilesWithGarrison = player.Map.Where(b => garrisonSystem.GetGarrisonedUnits(b).Count > 0);

        if (UnspentOnly)
            tilesWithGarrison = tilesWithGarrison
                .Where(b => garrisonSystem.GetGarrisonedUnits(b)
                    .Except(resourceGatheringSystem.GetSpentVillagers(card.Owner.Id)).Any());

        return tilesWithGarrison
            .Select(c => c.MapPosition)
            .ToList();
    }
}
