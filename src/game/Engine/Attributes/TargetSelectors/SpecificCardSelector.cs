using System.Collections.Generic;
using System.Linq;
using Godot;
using MedievalConquerors.Editors;
using MedievalConquerors.Editors.CustomEditors.ValueEditors;
using MedievalConquerors.Engine.Core;
using MedievalConquerors.Engine.Data;

namespace MedievalConquerors.Engine.Attributes.TargetSelectors;

public record SpecificCardSelector : TargetSelector
{
    [UseValueEditor(typeof(CardIdSelector))]
    public int SpecificCardId { get; init; }
    public int Range { get; init; }

    protected override List<Vector2I> SelectTargets(IGame game, Card card)
    {
        var player = card.Owner;
        var map = game.GetComponent<HexMap>();

        var matchingCards = player.Map.Where(c => c.Data.Id == SpecificCardId)
            .Select(c => c.MapPosition)
            .ToList();

        return Range <= 0
            ? matchingCards
            : matchingCards.SelectMany(c => map.GetReachable(c, Range)).Distinct().ToList();
    }
}
