using Godot;
using MedievalConquerors.Engine.Data;

namespace MedievalConquerors.Godot.Resources;

[GlobalClass]
public partial class Player : Resource, IPlayer
{
    [Export] private string _name;
}