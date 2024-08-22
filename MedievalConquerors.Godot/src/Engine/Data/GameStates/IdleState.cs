using MedievalConquerors.Engine.Core;
using MedievalConquerors.Engine.GameComponents;
using MedievalConquerors.Engine.StateManagement;

namespace MedievalConquerors.Engine.Data.GameStates;

public class IdleState : IState
{
    private readonly CardSystem _cardSystem;

    public IdleState(IGame game)
    {
        _cardSystem = game.GetComponent<CardSystem>();
    }
    
    public void Enter()
    {
        _cardSystem.Refresh();
        // TODO: Check AI System here to perform enemy action
    }
    public void Exit() { }
}