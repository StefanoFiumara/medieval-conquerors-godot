using MedievalConquerors.Engine.Actions;
using MedievalConquerors.Engine.Core;
using MedievalConquerors.Engine.Data;
using MedievalConquerors.Engine.Events;
using MedievalConquerors.Extensions;

namespace MedievalConquerors.Engine.GameComponents;

public class TurnSystem : GameComponent, IAwake
{
    private Match _match;
    private IEventAggregator _events;
    private IGameSettings _settings;

    public void Awake()
    {
        _match = Game.GetComponent<Match>();
        _events = Game.GetComponent<EventAggregator>();
        _settings = Game.GetComponent<IGameSettings>();
            
        _events.Subscribe<ChangeTurnAction>(GameEvent.Perform<ChangeTurnAction>(), OnPerformChangeTurn);
        _events.Subscribe<BeginGameAction>(GameEvent.Perform<BeginGameAction>(), OnPerformBeginGame);
    }

    private void OnPerformBeginGame(BeginGameAction action)
    {
        Game.AddReaction(new ShuffleDeckAction(Match.LocalPlayerId));
        Game.AddReaction(new ShuffleDeckAction(Match.EnemyPlayerId));
        
        // TODO: Formalize town center card data location

        var townCenter1 = CardBuilder.Build(_match.LocalPlayer)
            .WithTitle("Town Center")
            .WithDescription(
                "The center of your empire, gathers food from surrounding farms. If this building is destroyed, you lose the game.")
            .WithCardType(CardType.Building)
            .WithImagePath("res://assets/portraits/town_center.png")
            .WithTokenImagePath("res://assets/tile_tokens/town_center.png")
            .WithTags(Tags.Economic | Tags.TownCenter)
            .WithGarrisonCapacity(capacity: 6)
            .WithResourceCollector(ResourceType.Food, 1, 15)
            .WithSpawnPoint(Tags.TownCenter)
            .Create();
            // With HealthPoints
        
            var townCenter2 = CardBuilder.Build(_match.EnemyPlayer)
                .WithTitle("Town Center")
                .WithDescription(
                    "The center of your empire, gathers food from surrounding farms. If this building is destroyed, you lose the game.")
                .WithCardType(CardType.Building)
                .WithImagePath("res://assets/portraits/town_center.png")
                .WithTokenImagePath("res://assets/tile_tokens/town_center.png")
                .WithTags(Tags.Economic | Tags.TownCenter)
                .WithGarrisonCapacity(capacity: 6)
                .WithResourceCollector(ResourceType.Food, 1, 15)
                .WithSpawnPoint(Tags.TownCenter)
                .Create();
            
        Game.AddReaction(new PlayCardAction(townCenter1, _match.LocalPlayer.TownCenter.Position));
        Game.AddReaction(new PlayCardAction(townCenter2, _match.EnemyPlayer.TownCenter.Position));
        
        Game.AddReaction(new DrawCardsAction(Match.LocalPlayerId, _settings.StartingHandCount));
        Game.AddReaction(new DrawCardsAction(Match.EnemyPlayerId, _settings.StartingHandCount));
        
        Game.AddReaction(new ChangeTurnAction(action.StartingPlayerId));
    }

    private void OnPerformChangeTurn(ChangeTurnAction action)
    {
        _match.CurrentPlayerId = action.NextPlayerId;
        Game.AddReaction(new DrawCardsAction(action.NextPlayerId, 1));
        Game.AddReaction(new CollectResourcesAction(action.NextPlayerId));
    }
}