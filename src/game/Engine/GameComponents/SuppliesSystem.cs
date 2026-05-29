using System.Linq;
using MedievalConquerors.Engine.Actions;
using MedievalConquerors.Engine.Attributes;
using MedievalConquerors.Engine.Core;
using MedievalConquerors.Engine.Data;
using MedievalConquerors.Engine.Events;

namespace MedievalConquerors.Engine.GameComponents;

public class SuppliesSystem : GameComponent, IAwake
{
    private EventAggregator _events;
    private Match _match;
    private GarrisonSystem _garrisonSystem;

    public void Awake()
    {
        _events = Game.GetComponent<EventAggregator>();
        _match = Game.GetComponent<Match>();
        _garrisonSystem = Game.GetComponent<GarrisonSystem>();

        _events.Subscribe<SuppliesCheckAction>(GameEvent.Perform<SuppliesCheckAction>(), OnPerformSuppliesCheck);
    }

    private void OnPerformSuppliesCheck(SuppliesCheckAction action)
    {
        var player = _match.Players[action.TargetPlayerId];

        if (RequiresSupplies(player))
            Game.AddReaction(new CreateCardAction(CardLibrary.SUPPLIES_ID, action.TargetPlayerId, Zone.Hand));
    }

    public bool RequiresSupplies(Player p)
    {
        var occupiedBuildings = p.Map
            .Where(c => _garrisonSystem.GetGarrisonedUnits(c).Count != 0)
            .ToList();

        bool isLowOnResources = !p.Resources.CanAfford(food: 2, wood: 2);
        bool hasFoodGatherers = occupiedBuildings
            .Any(c => c.GetAttribute<ResourceProviderAttribute>()?.Resource == ResourceType.Food);
        bool hasWoodGatherers = occupiedBuildings
            .Any(c => c.GetAttribute<ResourceProviderAttribute>()?.Resource == ResourceType.Wood);

        return isLowOnResources && (!hasFoodGatherers || !hasWoodGatherers);
    }
}
