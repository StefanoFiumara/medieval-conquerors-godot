using Godot;
using MedievalConquerors.Engine.Core;

namespace MedievalConquerors.Engine.GameComponents;

public interface IGameSettings : IGameComponent
{
	int StartingHandCount { get; }
	bool DebugMode { get; }
	int StartingFoodCount { get; }
	int StartingWoodCount { get; }
	int StartingGoldCount { get; }
	int StartingStoneCount { get; }
}

[GlobalClass]
public partial class GameSettings : Resource, IGameSettings
{
	public IGame Game { get; set; }
	
	[Export] public int StartingHandCount { get; set; }
	[Export] public bool DebugMode { get; set; }
	
	[Export] public int StartingFoodCount  { get; set; }
	[Export] public int StartingWoodCount  { get; set; }
	[Export] public int StartingGoldCount  { get; set; }
	[Export] public int StartingStoneCount { get; set; }
}
