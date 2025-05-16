using MedievalConquerors.Engine.Actions;
using MedievalConquerors.Engine.Core;
using MedievalConquerors.Engine.Data;
using MedievalConquerors.Engine.Events;

namespace MedievalConquerors.Engine.GameComponents;

public class BuildingSystem : GameComponent, IAwake
{
    private EventAggregator _events;
    private HexMap _map;
    private CardLibrary _library;
    private Match _match;

    public void Awake()
    {
        _library = Game.GetComponent<CardLibrary>();
        _match = Game.GetComponent<Match>();
        _events = Game.GetComponent<EventAggregator>();
        _map = Game.GetComponent<HexMap>();

        _events.Subscribe<BuildStructureByIdAction, ActionValidatorResult>(GameEvent.Validate<BuildStructureByIdAction>(), OnValidateBuildStructureById);
        _events.Subscribe<BuildStructureAction>(GameEvent.Perform<BuildStructureAction>(), OnPerformBuildStructure);
        _events.Subscribe<BuildStructureByIdAction>(GameEvent.Perform<BuildStructureByIdAction>(), OnPerformBuildStructureById);
    }

    private void OnValidateBuildStructureById(BuildStructureByIdAction action, ActionValidatorResult validator)
    {
        if(!_library.IsValidCardId(action.CardId))
            validator.Invalidate($"{action.CardId} is not a valid card ID.");

        if(_map.GetTile(action.TargetTile) == null)
            validator.Invalidate("Invalid target tile.");
    }

    private void OnPerformBuildStructureById(BuildStructureByIdAction action)
    {
        var card = _library.LoadCard(action.CardId, _match.Players[action.OwnerId]);
        var reaction = new BuildStructureAction(card, action.TargetTile);
        Game.AddReaction(reaction);
    }

    private void OnPerformBuildStructure(BuildStructureAction action)
    {
        var tile = _map.GetTile(action.TargetTile);
        var player = action.StructureToBuild.Owner;

        tile.Building = action.StructureToBuild;
        action.StructureToBuild.MapPosition = tile.Position;
        player.MoveCard(action.StructureToBuild, Zone.Map);
    }
}
