using MedievalConquerors.Engine.Actions;
using MedievalConquerors.Engine.Core;
using MedievalConquerors.Engine.Data;
using MedievalConquerors.Engine.Events;
using MedievalConquerors.Engine.Logging;

namespace MedievalConquerors.Engine.GameComponents;

// TODO: Unit tests for this system once the logic is solidified
public class ResourceGatheringSystem : GameComponent, IAwake
{
    private EventAggregator _events;
    private Match _match;
    private HexMap _map;
    private GarrisonSystem _garrisonSystem;
    private ILogger _logger;

    public void Awake()
    {
        _match = Game.GetComponent<Match>();
        _map = Game.GetComponent<HexMap>();
        _events = Game.GetComponent<EventAggregator>();
        _logger = Game.GetComponent<ILogger>();

        _garrisonSystem = Game.GetComponent<GarrisonSystem>();


        _events.Subscribe<CollectResourcesAction>(GameEvent.Perform<CollectResourcesAction>(), OnPerformCollectResource);
    }

    private void OnPerformCollectResource(CollectResourcesAction action)
    {
        var player = _match.Players[action.TargetPlayerId];
        player.Resources[action.Resource] += action.Amount;
    }
}
