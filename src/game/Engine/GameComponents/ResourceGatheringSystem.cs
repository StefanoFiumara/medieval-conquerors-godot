using MedievalConquerors.Engine.Core;
using MedievalConquerors.Engine.Data;
using MedievalConquerors.Engine.Events;

namespace MedievalConquerors.Engine.GameComponents;

// TODO: Unit tests for this system once the logic is solidified
public class ResourceGatheringSystem : GameComponent, IAwake
{
    private EventAggregator _events;
    private Match _match;
    private HexMap _map;
    private GarrisonSystem _garrisonSystem;

    public void Awake()
    {
        _match = Game.GetComponent<Match>();
        _map = Game.GetComponent<HexMap>();
        _events = Game.GetComponent<EventAggregator>();

        _garrisonSystem = Game.GetComponent<GarrisonSystem>();
    }
}
