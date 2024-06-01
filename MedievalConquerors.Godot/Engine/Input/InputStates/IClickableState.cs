using Godot;
using MedievalConquerors.Engine.Data;
using MedievalConquerors.Engine.StateManagement;

namespace MedievalConquerors.Engine.Input.InputStates;

public interface IClickableState : IState
{
    IClickableState OnReceivedInput(IClickable clickedObject, InputEventMouseButton mouseEvent);
}