using System.Collections.Generic;
using System.Linq;
using MedievalConquerors.Engine.Actions;
using MedievalConquerors.Engine.Core;
using MedievalConquerors.Engine.Data;
using MedievalConquerors.Engine.Events;
using MedievalConquerors.Engine.Logging;

namespace MedievalConquerors.Engine.GameComponents;

public class ResourceGatheringSystem : GameComponent, IAwake
{
    private EventAggregator _events;
    private Match _match;
    private HexMap _map;
    private GarrisonSystem _garrisonSystem;
    private ILogger _logger;

    private readonly Dictionary<int, List<Card>> _spentVillagers = [];

    public IReadOnlyList<Card> GetSpentVillagers(int playerId) => _spentVillagers[playerId].AsReadOnly();

    public void Awake()
    {
        _match = Game.GetComponent<Match>();
        _map = Game.GetComponent<HexMap>();
        _events = Game.GetComponent<EventAggregator>();
        _logger = Game.GetComponent<ILogger>();

        _garrisonSystem = Game.GetComponent<GarrisonSystem>();

        _spentVillagers.Clear();
        _spentVillagers.Add(_match.LocalPlayer.Id, []);
        _spentVillagers.Add(_match.EnemyPlayer.Id, []);

        _events.Subscribe<BeginTurnAction>(GameEvent.Perform<BeginTurnAction>(), OnPerformBeginTurn);
        _events.Subscribe<ResetSpentVillagersAction>(GameEvent.Perform<ResetSpentVillagersAction>(), OnPerformResetSpentVillagers);

        // TODO: Target selector validates that this action is always valid
        // BUT we probably need to add a proper validation for this action in case it can be triggered in the future
        // outside of the input system.
        _events.Subscribe<CollectResourcesAction>(GameEvent.Perform<CollectResourcesAction>(), OnPerformCollectResource);
    }

    private void OnPerformBeginTurn(BeginTurnAction action) => Game.AddReaction(new ResetSpentVillagersAction(action.PlayerId));
    private void OnPerformResetSpentVillagers(ResetSpentVillagersAction action) => _spentVillagers[action.PlayerId].Clear();
    private void OnPerformCollectResource(CollectResourcesAction action)
    {
        var player = _match.Players[action.TargetPlayerId];

        var resource = action.Resource switch
        {
            ResourceType.Mining => DetermineMiningResource(action),
            _ => action.Resource
        };

        player.Resources[resource] += action.Amount;

        var building = _map.GetTile(action.TargetTile).Building;
        var villager = _garrisonSystem.GetGarrisonedUnits(building)
            .Except(_spentVillagers[action.TargetPlayerId])
            .First();

        _spentVillagers[action.TargetPlayerId].Add(villager);
    }

    private ResourceType DetermineMiningResource(CollectResourcesAction action)
    {
        var neighbors = _map.GetNeighbors(action.TargetTile).ToList();

        if (neighbors.Any(t => t.ResourceType == ResourceType.Gold))
            return ResourceType.Gold;

        if (neighbors.Any(t => t.ResourceType == ResourceType.Stone))
            return ResourceType.Stone;

        // TODO: What if we have both gold and stone neighbors?
        // var goldDeposits = neighbors.Count(n => n.ResourceType == ResourceType.Gold);
        // var stoneDeposits = neighbors.Count(n => n.ResourceType == ResourceType.Stone);

        return ResourceType.None;

    }
}
