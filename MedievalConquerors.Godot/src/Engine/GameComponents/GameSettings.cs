using Godot;
using MedievalConquerors.Engine.Core;

namespace MedievalConquerors.Engine.GameComponents;

public interface IGameSettings : IGameComponent
{
	int StartingHandCount { get; }
	bool DebugMode { get; }
}

[GlobalClass]
public partial class GameSettings : Resource, IGameSettings
{
	public IGame Game { get; set; }
	
	[Export]
	public int StartingHandCount { get; set; }
	
	[Export]
	public bool DebugMode { get; set; }
}
