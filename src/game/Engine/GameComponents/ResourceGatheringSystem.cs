using System.Collections.Generic;
using System.Linq;
using MedievalConquerors.Engine.Actions;
using MedievalConquerors.Engine.Attributes;
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

	private readonly Dictionary<int, List<Card>> _spentVillagers = [];

	public IReadOnlyList<Card> GetSpentVillagers(int playerId) => _spentVillagers[playerId].AsReadOnly();

	public void Awake()
	{
		_match = Game.GetComponent<Match>();
		_map = Game.GetComponent<HexMap>();
		_events = Game.GetComponent<EventAggregator>();

		_garrisonSystem = Game.GetComponent<GarrisonSystem>();

		_spentVillagers.Clear();
		_spentVillagers.Add(_match.LocalPlayer.Id, []);
		_spentVillagers.Add(_match.EnemyPlayer.Id, []);

		_events.Subscribe<BeginTurnAction>(GameEvent.Perform<BeginTurnAction>(), OnPerformBeginTurn);
		_events.Subscribe<ResetSpentVillagersAction>(GameEvent.Perform<ResetSpentVillagersAction>(), OnPerformResetSpentVillagers);

		_events.Subscribe<PassiveResourceCollectionAction>(GameEvent.Perform<PassiveResourceCollectionAction>(), OnPerformCollectResources);
		_events.Subscribe<HarvestAction, ActionValidatorResult>(GameEvent.Validate<HarvestAction>(), OnValidateCollectResources);
		_events.Subscribe<HarvestAction>(GameEvent.Perform<HarvestAction>(), OnPerformCollectResource);
	}

	private void OnPerformCollectResources(PassiveResourceCollectionAction action)
	{
		// TODO: Animation for this step (use popup labels like before)
		var player = _match.Players[action.PlayerId];

		var villagers = player.Map
			.SelectMany(c => _garrisonSystem.GetGarrisonedUnits(c))
			.Distinct()
			.Select(c => (villager: c, building: _map.GetTile(c.MapPosition).Building));

		foreach (var (_, building) in villagers)
		{
			var resourceProvider = building.GetAttribute<ResourceProviderAttribute>();
			var resource = DetermineResource(resourceProvider.Resource, building);

			player.Resources[resource] += resourceProvider.PassiveYield;
		}
	}

	private void OnValidateCollectResources(HarvestAction action, ActionValidatorResult validator)
	{
		var tile = _map.GetTile(action.TargetTile);
		if (tile.Building == null)
		{
			validator.Invalidate("No building on target tile.");
			return;
		}

		if (!tile.Building.HasAttribute<GarrisonCapacityAttribute>())
		{
			validator.Invalidate("Targeted building does not have Garrison Capacity");
			return;
		}

		if (!tile.Building.HasAttribute<ResourceProviderAttribute>())
		{
			validator.Invalidate("Targeted building does not have Resource Provider");
			return;
		}

		if (!_garrisonSystem.GetGarrisonedUnits(tile.Building).Except(_spentVillagers[action.TargetPlayerId]).Any())
		{
			validator.Invalidate("Targeted building has no unspent garrison slots");
		}
	}

	private void OnPerformBeginTurn(BeginTurnAction action)
		=> Game.AddReaction(new ResetSpentVillagersAction(action.PlayerId));

	private void OnPerformResetSpentVillagers(ResetSpentVillagersAction action)
		=> _spentVillagers[action.PlayerId].Clear();

	private void OnPerformCollectResource(HarvestAction action)
	{
		var player = _match.Players[action.TargetPlayerId];
		var building = _map.GetTile(action.TargetTile).Building;

		var resourceProvider = building.GetAttribute<ResourceProviderAttribute>();
		var resource = DetermineResource(resourceProvider.Resource, building);

		player.Resources[resource] += resourceProvider.HarvestYield;

		var villager = _garrisonSystem.GetGarrisonedUnits(building)
			.Except(_spentVillagers[action.TargetPlayerId])
			.First();

		_spentVillagers[action.TargetPlayerId].Add(villager);
	}

	private ResourceType DetermineResource(ResourceType resource, Card building)
	{
		var result = resource;
		if (resource == ResourceType.Mining)
		{
			// determine mining type based on adjacent tile
			// TODO: what happens if we have both?
			var neighbors = _map.GetNeighbors(building.MapPosition).ToList();
			if (neighbors.Any(t => t.ResourceType == ResourceType.Gold))
				result = ResourceType.Gold;
			else if (neighbors.Any(t => t.ResourceType == ResourceType.Stone))
				result = ResourceType.Stone;
		}

		return result;
	}
}
