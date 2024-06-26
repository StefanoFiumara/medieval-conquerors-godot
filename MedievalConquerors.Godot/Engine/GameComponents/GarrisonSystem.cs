using MedievalConquerors.Engine.Actions;
using MedievalConquerors.Engine.Core;
using MedievalConquerors.Engine.Data.Attributes;
using MedievalConquerors.Engine.Events;
using MedievalConquerors.Extensions;

namespace MedievalConquerors.Engine.GameComponents;

// TODO: Unit tests for this system
public class GarrisonSystem : GameComponent, IAwake
{
    private IEventAggregator _events;

    public void Awake()
    {
        _events = Game.GetComponent<EventAggregator>();
        _events.Subscribe<GarrisonAction>(GameEvent.Perform<GarrisonAction>(), OnPerformGarrison);
        _events.Subscribe<GarrisonAction, ActionValidatorResult>(GameEvent.Validate<GarrisonAction>(), OnValidateGarrison);
    }

    private void OnValidateGarrison(GarrisonAction action, ActionValidatorResult validator)
    {
        var garrisonAttribute = action.Building.GetAttribute<GarrisonCapacityAttribute>();
        if (garrisonAttribute == null)
        {
            validator.Invalidate("This building cannot garrison any units.");
            return;
        }
        
        if(!garrisonAttribute.CanGarrison(action.Unit))
            validator.Invalidate("This building cannot garrison the given unit.");
    }

    private void OnPerformGarrison(GarrisonAction action)
    {
        var garrisonAttribute = action.Building.GetAttribute<GarrisonCapacityAttribute>();
        garrisonAttribute.Garrison(action.Unit);
    }
}