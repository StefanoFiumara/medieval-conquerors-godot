namespace MedievalConquerors.Engine.StateManagement;

public interface IState
{
    void Enter();
    void Exit();
}
    
public class NullGameState : IState
{
    public void Enter()
    {
            
    }

    public void Exit()
    {
            
    }
}