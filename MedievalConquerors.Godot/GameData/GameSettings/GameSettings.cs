using Godot;
using MedievalConquerors.Engine.Core;
using MedievalConquerors.Engine.GameComponents;

namespace MedievalConquerors.GameData.GameSettings;


[GlobalClass]
public partial class GameSettings : Resource, IGameSettings
{
	public IGame Game { get; set; }
}
