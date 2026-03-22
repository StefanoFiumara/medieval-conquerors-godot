using System.Collections.Generic;
using System.Linq;
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

    private readonly Dictionary<int, List<Card>> _usedVillagers = [];

    public IReadOnlyList<Card> GetUsedVillagers(int playerId) => _usedVillagers[playerId].AsReadOnly();

    public void Awake()
    {
        _match = Game.GetComponent<Match>();
        _map = Game.GetComponent<HexMap>();
        _events = Game.GetComponent<EventAggregator>();
        _logger = Game.GetComponent<ILogger>();

        _garrisonSystem = Game.GetComponent<GarrisonSystem>();

        _usedVillagers.Clear();
        _usedVillagers.Add(_match.LocalPlayer.Id, []);
        _usedVillagers.Add(_match.EnemyPlayer.Id, []);

        _events.Subscribe<BeginTurnAction>(GameEvent.Perform<BeginTurnAction>(), OnPerformBeginTurn);
        _events.Subscribe<CollectResourcesAction>(GameEvent.Perform<CollectResourcesAction>(), OnPerformCollectResource);
    }

    private void OnPerformBeginTurn(BeginTurnAction action)
    {
        _usedVillagers[action.PlayerId].Clear();
    }

    private void OnPerformCollectResource(CollectResourcesAction action)
    {
        var player = _match.Players[action.TargetPlayerId];
        player.Resources[action.Resource] += action.Amount;

        var building = _map.GetTile(action.TargetTile).Building;
        var villager = _garrisonSystem.GetGarrisonedUnits(building)
            .Except(_usedVillagers[action.TargetPlayerId])
            .First();

        // TODO: UI Indicator to show this villager token was spent.
        _usedVillagers[action.TargetPlayerId].Add(villager);
    }
}
