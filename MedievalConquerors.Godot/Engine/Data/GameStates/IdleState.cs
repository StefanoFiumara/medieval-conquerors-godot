using MedievalConquerors.Engine.Core;
using MedievalConquerors.Engine.GameComponents;
using MedievalConquerors.Engine.StateManagement;

namespace MedievalConquerors.Engine.Data.GameStates;

public class IdleState : IState
{
    private readonly IGame _game;

    public IdleState(IGame game)
    {
        _game = game;
    }
    
    public void Enter()
    {
        _game.GetComponent<CardSystem>().Refresh();
        
        // TODO: Check AI System here to perform enemy action
        
    }
    public void Exit() { }
}