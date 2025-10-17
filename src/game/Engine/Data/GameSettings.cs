using Godot;
using MedievalConquerors.Engine.Core;

namespace MedievalConquerors.Engine.Data;

public interface IGameSettings : IGameComponent
{
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

	[Export] public bool DebugMode { get; set; }

	[Export] public int StartingFoodCount  { get; set; }
	[Export] public int StartingWoodCount  { get; set; }
	[Export] public int StartingGoldCount  { get; set; }
	[Export] public int StartingStoneCount { get; set; }
}
