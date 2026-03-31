using Godot;
using MedievalConquerors.Engine.Core;

namespace MedievalConquerors.Engine.Data;

public interface IGameSettings : IGameComponent
{
	bool TestMode { get; }
	bool DebugDrawHoverSections { get; }
	bool DebugShowTileCoords { get; }
	int StartingFoodCount { get; }
	int StartingWoodCount { get; }
	int StartingGoldCount { get; }
	int StartingStoneCount { get; }
}

[GlobalClass]
public partial class GameSettings : Resource, IGameSettings
{
	public IGame Game { get; set; }

	[Export] public bool TestMode { get; set; }
	[Export] public bool DebugDrawHoverSections { get; set; }
	[Export] public bool DebugShowTileCoords { get; set; }

	[Export] public int StartingFoodCount  { get; set; }
	[Export] public int StartingWoodCount  { get; set; }
	[Export] public int StartingGoldCount  { get; set; }
	[Export] public int StartingStoneCount { get; set; }
}
