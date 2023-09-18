using Godot;
using MedievalConquerors.Engine.Data;

namespace MedievalConquerors.GameData.Players;

[GlobalClass]
public partial class Player : Resource, IPlayer
{
    [Export] private string _name;
}