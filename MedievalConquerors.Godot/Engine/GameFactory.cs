using MedievalConquerors.Engine.Core;
using MedievalConquerors.Engine.Data;
using MedievalConquerors.Engine.Events;
using MedievalConquerors.Engine.GameComponents;
using MedievalConquerors.Engine.Input;
using MedievalConquerors.Engine.Logging;

namespace MedievalConquerors.Engine;

public static class GameFactory
{
    public static Game Create(ILogger logger, IGameMap map, IGameSettings settings)
    {
        var game = new Game();
        
        game.AddComponent(logger);
        game.AddComponent(map);
        game.AddComponent(settings);
        
        game.AddComponent<Match>();
        
        game.AddComponent<EventAggregator>();
        game.AddComponent<ActionSystem>();
        
        game.AddComponent<HandSystem>();
        game.AddComponent<PlayerSystem>();
        game.AddComponent<MapSystem>();

        game.AddComponent<TurnSystem>();
        game.AddComponent<InputSystem>();

        game.AddComponent<CardSystem>();
        game.AddComponent<GlobalGameStateSystem>();
        // game.AddComponent<VictorySystem>();
        
        game.AddComponent<ResourceSystem>();
        // game.AddComponent<AgeSystem>();
        
        // // TODO: Only add AISystem when in Single Player Mode, otherwise, add "NetworkSystem" for multiplayer
        // game.AddComponent<AISystem>();
        
        // game.AddComponent<TargetSystem>();
        
        // game.AddComponent<DiscardSystem>();
        
        // game.AddComponent<CombatSystem>();
        
        // game.AddComponent<UniquenessSystem>();
        
        // game.AddComponent<AbilitySystem>();
        
        return game;
    }
}