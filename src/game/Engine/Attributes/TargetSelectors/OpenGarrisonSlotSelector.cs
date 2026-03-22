using System.Collections.Generic;
using System.Linq;
using Godot;
using MedievalConquerors.Engine.Core;
using MedievalConquerors.Engine.Data;
using MedievalConquerors.Engine.GameComponents;

namespace MedievalConquerors.Engine.Attributes.TargetSelectors;

public record OpenGarrisonSlotSelector : TargetSelector
{

    protected override List<Vector2I> SelectTargets(IGame game, Card card)
    {
        var garrisonSystem = game.GetComponent<GarrisonSystem>();
        return card.Owner.Map
            .Where(c => c.Data.CardType == CardType.Building)
            .Where(b => garrisonSystem.CanGarrison(b, card))
            .Select(b => b.MapPosition)
            .ToList();
    }
}