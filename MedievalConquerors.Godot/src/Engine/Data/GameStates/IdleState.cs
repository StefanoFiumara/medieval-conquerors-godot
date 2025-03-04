using MedievalConquerors.Engine.Core;
using MedievalConquerors.Engine.GameComponents;
using MedievalConquerors.Engine.StateManagement;

namespace MedievalConquerors.Engine.Data.GameStates;

public class IdleState(IGame game) : IState
{
    private readonly Match _match = game.GetComponent<Match>();
    private readonly CardSystem _cardSystem = game.GetComponent<CardSystem>();
    private readonly AISystem _aiSystem = game.GetComponent<AISystem>();

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
