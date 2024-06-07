using Godot;
using MedievalConquerors.Engine.Core;
using MedievalConquerors.Engine.Data;
using MedievalConquerors.Engine.GameComponents;
using MedievalConquerors.Views.Entities;
using TileData = MedievalConquerors.Engine.Data.TileData;

namespace MedievalConquerors.Engine.Input.InputStates;

public abstract class BaseInputState : IClickableState
{
    public abstract void Enter();
    public abstract void Exit();
    public abstract IClickableState OnReceivedInput(IClickable clickedObject, InputEventMouseButton mouseEvent);
    
    protected IGame Game { get; }
    
    private readonly CardSystem _cardSystem;
    private readonly Match _match;

    protected BaseInputState(IGame game)
    {
        Game = game;
        _cardSystem = game.GetComponent<CardSystem>();
        _match = game.GetComponent<Match>();
    }


    protected bool IsPlayableCard(IClickable obj) => obj is CardView view && _cardSystem.IsPlayable(view.Card);

    protected bool IsOwnedUnit(IClickable obj) => obj is TileData t && t.Unit != null && t.Unit.Owner == _match.LocalPlayer;
    
    protected bool IsEnemyUnit(IClickable obj) => obj is TileData t && t.Unit != null && t.Unit.Owner == _match.EnemyPlayer;


}