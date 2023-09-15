﻿using MedievalConquerors.Engine.Core;
using MedievalConquerors.Engine.Data;
using MedievalConquerors.Engine.Events;
using MedievalConquerors.Engine.GameComponents;
using MedievalConquerors.Engine.Logging;

namespace MedievalConquerors.Engine;

public static class GameFactory
{
    public static Game Create(ILogger logger, IMatch match, IGameSettings settings)
    {
        var game = new Game();

        game.AddComponent(logger);
        game.AddComponent(match);
        game.AddComponent(settings);

        game.AddComponent<EventAggregator>();
        game.AddComponent<ActionSystem>();
        
        // game.AddComponent<VictorySystem>();
        //
        // game.AddComponent<GlobalGameStateSystem>();
        //
        // game.AddComponent<PlayerSystem>();
        // game.AddComponent<TurnSystem>();
        //
        // game.AddComponent<ResourceSystem>();
        // game.AddComponent<AgeSystem>();
        //
        // // // TODO: Only add AISystem when in Single Player Mode, otherwise, add NetworkSystem for multiplayer
        // game.AddComponent<AISystem>();
        // //
        // game.AddComponent<CardSystem>();
        // game.AddComponent<TargetSystem>();
        //
        // game.AddComponent<HandSystem>();
        // game.AddComponent<BoardSystem>();
        // game.AddComponent<DiscardSystem>();
        //
        // game.AddComponent<CombatSystem>();
        //
        // game.AddComponent<UniquenessSystem>();
        //
        // game.AddComponent<AbilitySystem>();
        
        return game;
    }
}