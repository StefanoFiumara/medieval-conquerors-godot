using MedievalConquerors.Engine.Data;
using MedievalConquerors.Engine.StateManagement;

namespace MedievalConquerors.Engine.Input.InputStates;

public interface ITurnState : IState
{
    ITurnState OnReceivedInput(IGameObject clickedObject);
}