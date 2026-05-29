using System;
using MedievalConquerors.Engine.Attributes;


namespace MedievalConquerors.Engine.Data;

public class ResourceBank
{
	public int Food { get; private set; }
	public int Wood { get; private set; }
	public int Gold { get; private set; }
	public int Stone { get; private set; }

	public int TotalResources => Food + Wood + Gold + Stone;

	public bool CanAfford(ResourceCostAttribute attr) => CanAfford(attr.Food, attr.Wood, attr.Gold, attr.Stone);
	public bool CanAfford(int food = 0, int wood = 0, int gold = 0, int stone = 0)
		=> Food  >= food
		&& Wood  >= wood
		&& Gold  >= gold
		&& Stone >= stone;

	public void Subtract(ResourceCostAttribute resourceCost)
	{
		if (!CanAfford(resourceCost))
			throw new ArgumentException("Cannot afford resource cost");

		Food  -= resourceCost.Food;
		Wood  -= resourceCost.Wood;
		Gold  -= resourceCost.Gold;
		Stone -= resourceCost.Stone;
	}

	public int this[ResourceType resourceType]
	{
		get
		{
			return resourceType switch
			{
				ResourceType.Food => Food,
				ResourceType.Wood => Wood,
				ResourceType.Gold => Gold,
				ResourceType.Stone => Stone,
				_ => 0
			};
		}
		set
		{
			switch (resourceType)
			{
				case ResourceType.Food:
					Food = value;
					break;
				case ResourceType.Wood:
					Wood = value;
					break;
				case ResourceType.Gold:
					Gold = value;
					break;
				case ResourceType.Stone:
					Stone = value;
					break;
				default:
					throw new ArgumentException("Invalid resource type");
			}
		}
	}
}
