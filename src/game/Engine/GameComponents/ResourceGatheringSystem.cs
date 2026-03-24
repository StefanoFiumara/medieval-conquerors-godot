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

		// TODO: Target selector validates that this action is always valid
		// BUT we probably need to add a proper validation for this action in case it can be triggered in the future
		// outside of the input system.
		// VALIDATION: Target tile has building
		//             Target building has unspent garrison slots
		//             Target building has Resource Provider attribute
		_events.Subscribe<CollectResourcesAction, ActionValidatorResult>(GameEvent.Validate<CollectResourcesAction>(), OnValidateCollectResources);
		_events.Subscribe<CollectResourcesAction>(GameEvent.Perform<CollectResourcesAction>(), OnPerformCollectResource);
	}

	private void OnValidateCollectResources(CollectResourcesAction action, ActionValidatorResult validator)
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

	private void OnPerformCollectResource(CollectResourcesAction action)
	{
		var player = _match.Players[action.TargetPlayerId];
		var building = _map.GetTile(action.TargetTile).Building;

		var resource = building.GetAttribute<ResourceProviderAttribute>().Resource;
		if (resource == ResourceType.Mining)
		{
			// determine mining type based on adjacent tile
			// TODO: what happens if we have both?
			var neighbors = _map.GetNeighbors(building.MapPosition).ToList();
			if (neighbors.Any(t => t.ResourceType == ResourceType.Gold))
				resource = ResourceType.Gold;
			else if (neighbors.Any(t => t.ResourceType == ResourceType.Stone))
				resource = ResourceType.Stone;
		}

		player.Resources[resource] += action.Amount;

		var villager = _garrisonSystem.GetGarrisonedUnits(building)
			.Except(_spentVillagers[action.TargetPlayerId])
			.First();

		_spentVillagers[action.TargetPlayerId].Add(villager);
	}
}
