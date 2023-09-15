using Godot;
using MedievalConquerors.Engine.Core;
using MedievalConquerors.Engine.Data;
using MedievalConquerors.Engine.GameComponents;

namespace MedievalConquerors.Godot.Resources;

[GlobalClass]
public partial class Match : Resource, IMatch
{
    [Export] private GameBoard _gameBoard;
    [Export] private Player _localPlayer;
    [Export] private Player _enemyPlayer;
    
    public IGameBoard GameBoard => _gameBoard;
    public IPlayer LocalPlayer => _localPlayer;
    public IPlayer EnemyPlayer => _enemyPlayer;
    
    public IGame Game { get; set; }
}