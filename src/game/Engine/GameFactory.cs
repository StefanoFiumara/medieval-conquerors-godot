using MedievalConquerors.Engine.Core;
using MedievalConquerors.Engine.Data;
using MedievalConquerors.Engine.Events;
using MedievalConquerors.Engine.GameComponents;
using MedievalConquerors.Engine.Input;
using MedievalConquerors.Engine.Logging;

namespace MedievalConquerors.Engine;

public static class GameFactory
{
    public static Game Create(ILogger logger, HexMap map, IGameSettings settings, CardLibrary library = null)
    {
        var game = new Game();

        game.AddComponent(logger);
        game.AddComponent(map);
        game.AddComponent(settings);

        game.AddComponent<Match>();

        if(library != null)
            game.AddComponent(library);
        else
            game.AddComponent<CardLibrary>();

        game.AddComponent<EventAggregator>();
        game.AddComponent<ActionSystem>();

        game.AddComponent<HandSystem>();
        game.AddComponent<PlayerSystem>();
        game.AddComponent<MapSystem>();

        game.AddComponent<TurnSystem>();
        game.AddComponent<TargetSystem>();

        game.AddComponent<DeckCycleSystem>();
        game.AddComponent<CardSystem>();
        game.AddComponent<CardCreationSystem>();
        game.AddComponent<GlobalGameStateSystem>();
        game.AddComponent<AbilitySystem>();
        // game.AddComponent<VictorySystem>();

        game.AddComponent<VillagerSystem>();
        game.AddComponent<ResourceGatheringSystem>();
        game.AddComponent<ResourceCostSystem>();
        game.AddComponent<BuildingSystem>();
        game.AddComponent<MovementSystem>();
        game.AddComponent<GarrisonSystem>();

        // TODO: Only add AISystem when in Single Player Mode, otherwise, add "NetworkSystem" for multiplayer
        game.AddComponent<AISystem>();


        return game;
    }
}
