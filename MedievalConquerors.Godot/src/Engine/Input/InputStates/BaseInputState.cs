using Godot;
using MedievalConquerors.Engine.Core;
using MedievalConquerors.Engine.GameComponents;
using MedievalConquerors.Engine.StateManagement;
using MedievalConquerors.Views;
using TileData = MedievalConquerors.Engine.Data.TileData;

namespace MedievalConquerors.Engine.Input.InputStates;

public abstract class BaseInputState(IGame game) : IClickableState
{
    public abstract void Enter();
    public abstract void Exit();
    public abstract IClickableState OnReceivedInput(IClickable clickedObject, InputEventMouseButton mouseEvent);

    protected IGame Game { get; } = game;

    private readonly CardSystem _cardSystem = game.GetComponent<CardSystem>();
    private readonly Match _match = game.GetComponent<Match>();


    protected bool IsPlayableCard(IClickable obj) => obj is CardView view && _cardSystem.IsPlayable(view.Card);

    protected bool IsOwnedUnit(IClickable obj) => obj is TileData t && t.Unit != null && t.Unit.Owner == _match.LocalPlayer;

    protected bool IsEnemyUnit(IClickable obj) => obj is TileData t && t.Unit != null && t.Unit.Owner == _match.EnemyPlayer;
}

public interface IClickableState : IState
{
    IClickableState OnReceivedInput(IClickable clickedObject, InputEventMouseButton mouseEvent);
}
