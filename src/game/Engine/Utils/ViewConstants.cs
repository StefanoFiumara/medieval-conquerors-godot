using Godot;

namespace MedievalConquerors.Engine.Utils;

public static class ViewConstants
{
	public static readonly Vector2 ReferenceResolution = new(1920, 1080);
}

public static class ViewScalingExtensions
{
	public static Vector2 CalculateScaleFactor(this Rect2 visibleRect)
	{
		var proportion = visibleRect.Size / ViewConstants.ReferenceResolution;
		var newScaleFactor = Mathf.Min(proportion.X, proportion.Y);
		return new Vector2(newScaleFactor, newScaleFactor);
	}
}
