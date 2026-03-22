using System.Collections.Generic;
using Godot;
using MedievalConquerors.Engine.Attributes;
using MedievalConquerors.Engine.Core;
using MedievalConquerors.Engine.Data;
using MedievalConquerors.Engine.Input;

namespace MedievalConquerors.Entities.Tokens;

public partial class TokenView : Node2D, IClickable
{
	private static readonly Texture2D BUILDER_TOKEN = GD.Load<Texture2D>("uid://dagmpcs02rwxr");
	private const float BUILDER_TOKEN_RADIUS = 65f;

	private Sprite2D _badge;
	private Sprite2D _icon;

	private readonly List<Sprite2D> _builderTokens = [];

	public Card Card { get; private set; }

	public override void _EnterTree()
	{
		_badge = GetNode<Sprite2D>("%team_badge");
		_icon = GetNode<Sprite2D>("%token_icon");
	}

	public void Initialize(IGame game, Card card)
	{
		Card = card;

		// NOTE: The Match Player ID matches up to the frame we want to display for the badge:
		//		0 == Local Player
		//		1 == Enemy Player
		_badge.Frame = card.Owner.Id;
		_icon.Frame = card.Data.TokenFrameId;

		// TODO: Set up Garrison View
		// IDEA: Calculate builder token position in code?
		SetGarrisonView(0);
	}

	public void SetGarrisonView(int garrisonCount)
	{
		if (garrisonCount > _builderTokens.Count)
		{
			for (int i = 0; i < _builderTokens.Count; i++)
			{
				var token = _builderTokens[i];
				var tween = CreateTween().SetTrans(Tween.TransitionType.Sine).SetEase(Tween.EaseType.InOut).SetParallel();

				var targetPosition = GetRadialPosition(i, garrisonCount);
				tween.TweenInterval(i * 0.15);
				tween.Chain().TweenProperty(token, "position", targetPosition, 0.15);
			}

			for (int i = _builderTokens.Count; i < garrisonCount; i++)
			{
				var token = new Sprite2D()
				{
					Texture = BUILDER_TOKEN,
					Hframes = 2,
					Frame = Card.Owner.Id,
					Modulate = Colors.Transparent
				};

				token.Position = GetRadialPosition(i, garrisonCount);
				_builderTokens.Add(token);
				AddChild(token);

				var tween = CreateTween().SetTrans(Tween.TransitionType.Sine).SetEase(Tween.EaseType.In).SetParallel();

				tween.TweenInterval(i * 0.15);
				tween.Chain().TweenProperty(token, "modulate", Colors.White, 0.15);
			}
		}
		else if (garrisonCount < _builderTokens.Count)
		{
			var eraseCount = _builderTokens.Count - garrisonCount;
			var toErase = _builderTokens[^eraseCount ..];

			for (var i = 0; i < toErase.Count; i++)
			{
				var token = toErase[i];

				var tween = CreateTween().SetTrans(Tween.TransitionType.Sine).SetEase(Tween.EaseType.Out).SetParallel();
				tween.TweenInterval(i * 0.15);
				tween.Chain().TweenProperty(token, "modulate", Colors.Transparent, 0.15);

				tween.Chain().TweenCallback(Callable.From(() =>
				{
					_builderTokens.Remove(token);
					token.QueueFree();
				}));
			}
		}
	}

	private Vector2 GetRadialPosition(int idx, int totalCount)
	{
		const float SPREAD = Mathf.Pi * 0.5f;

		if (totalCount == 1)
			return new Vector2(0, - BUILDER_TOKEN_RADIUS);

		var arcPosition = (float)idx / (totalCount - 1);
		var angle = -Mathf.Pi / 2f + (arcPosition - 0.5f) * SPREAD;

		var x = BUILDER_TOKEN_RADIUS * Mathf.Cos(angle);
		var y = BUILDER_TOKEN_RADIUS * Mathf.Sin(angle);

		return new Vector2(x, y);
	}
}
