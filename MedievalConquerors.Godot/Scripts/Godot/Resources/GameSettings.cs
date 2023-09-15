using Godot;
using MedievalConquerors.Engine.Core;
using MedievalConquerors.Engine.Data;
using MedievalConquerors.Engine.GameComponents;

namespace MedievalConquerors.Godot.Resources;


[GlobalClass]
public partial class GameSettings : Resource, IGameSettings
{
    public IGame Game { get; set; }
}