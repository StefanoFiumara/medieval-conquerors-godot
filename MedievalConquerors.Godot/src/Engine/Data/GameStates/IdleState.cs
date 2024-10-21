using MedievalConquerors.Engine.Core;
using MedievalConquerors.Engine.GameComponents;
using MedievalConquerors.Engine.StateManagement;

namespace MedievalConquerors.Engine.Data.GameStates;

public class IdleState : IState
{
    private readonly Match _match;
    private readonly CardSystem _cardSystem;
    private readonly AISystem _aiSystem;

    public IdleState(IGame game)
    {
        _match = game.GetComponent<Match>();
        _cardSystem = game.GetComponent<CardSystem>();
        _aiSystem = game.GetComponent<AISystem>();
    }
    
    public void Enter()
    {
        _cardSystem.Refresh();
        if (_match.CurrentPlayerId == _match.EnemyPlayer.Id)
        {
            _aiSystem.UseAction();
        }
        
    }
    public void Exit() { }
}