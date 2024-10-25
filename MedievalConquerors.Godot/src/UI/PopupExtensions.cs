using Godot;
using MedievalConquerors.Engine.Data;
using MedievalConquerors.Extensions;

namespace MedievalConquerors.UI;

public static class PopupExtensions
{
	private static readonly PackedScene PopupScene = GD.Load<PackedScene>("res://scenes/ui/text_popup.tscn");
	
	public static Tween CreatePopup(
		this Node2D parent,
		Vector2 position,
		string text,
		double duration = 0.45,
		float textScale = 1f)
	{
		var tween = parent.CreateTween().SetParallel().SetTrans(Tween.TransitionType.Sine);

		var popup = PopupScene.Instantiate<PopupLabel>();
		parent.AddChild(popup);
		popup.Label.AppendText(text);
		popup.Position = position;
		popup.Scale = Vector2.One * textScale;
		tween.TweenProperty(popup, "position", position + Vector2I.Up * 40, duration);
		tween.TweenProperty(popup, "modulate:a", 0f, duration);
		tween.Chain().TweenCallback(Callable.From(() =>
		{
			popup.QueueFree();
		}));

		return tween;
	}
	
	public static Tween CreateResourcePopup(
		this Node2D parent, 
		Vector2 position, 
		ResourceType resource, 
		int amount, 
		double duration = 0.45)
	{
		var color = ResourceColors.Map[resource];
		var icon = ResourceIcons.Map[resource];
		string text = $"[color={color}]+{amount}[/color]{icon}";
		return CreatePopup(parent, position, text, duration);
	}
}
