using Godot;
using MedievalConquerors.Engine.Data;

namespace MedievalConquerors.GameData.Cards.Attributes;

[GlobalClass]
public partial class ResourceCost : Resource, ICardAttribute
{
	[Export] public int Food { get; private set; }
	[Export] public int Wood { get; private set; }
	[Export] public int Gold { get; private set; }
	[Export] public int Stone { get; private set; }
}
