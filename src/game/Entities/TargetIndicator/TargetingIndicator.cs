using System;
using System.Collections.Generic;
using Godot;

namespace MedievalConquerors.Entities;

public partial class TargetingIndicator : Node2D
{
	private static readonly PackedScene _arrowHead = GD.Load<PackedScene>("uid://c8gy3qfxua070");
	private static readonly PackedScene _arrowNode = GD.Load<PackedScene>("uid://bflq4f3cbiaet");

	[Export] private int _nodeCount;
	[Export] private float _scaleFactor;

	private List<Node2D> _nodes;
	private List<Vector2> _controlPoints;
	private Viewport _viewport;

	private readonly Vector2[] _controlPointFactors =
	[
		new(-0.3f, 0.8f),
		new(0.1f, 1.4f)
	];

	public bool IsTargeting
	{
		get;
		set
		{
			if (field != value)
			{
				field = value;
				if (!value)
				{
					for (int i = 0; i < _nodes.Count; i++)
						_nodes[i].Visible = false;
				}
				else
				{
					for (int i = 0; i < _nodes.Count; i++)
						_nodes[i].Visible = true;
				}
			}

		}
	}

	public override void _Ready()
	{
		_viewport = GetViewport();
		_nodes = new List<Node2D>(_nodeCount);
		for (int i = 0; i < _nodeCount; i++)
		{
			var node = _arrowNode.Instantiate<Node2D>();
			node.Visible = false;
			_nodes.Add(node);
			AddChild(node);
		}

		var head = _arrowHead.Instantiate<Node2D>();
		head.Visible = false;
		_nodes.Add(head);
		AddChild(head);

		_controlPoints = [Vector2.Zero, Vector2.Zero, Vector2.Zero, Vector2.Zero];
	}

	public override void _Process(double delta)
	{
		if (!IsTargeting) return;

		var mousePos = _viewport.GetMousePosition();

		_controlPoints[0] = GlobalPosition;
		_controlPoints[3] = mousePos;

		_controlPoints[1] = _controlPoints[0] + (_controlPoints[3] - _controlPoints[0]) * _controlPointFactors[0];
		_controlPoints[2] = _controlPoints[0] + (_controlPoints[3] - _controlPoints[0]) * _controlPointFactors[1];

		for (int i = 0; i < _nodes.Count; i++)
		{
			var t = MathF.Log2(1f * i / (_nodes.Count - 1) + 1f);
			_nodes[i].GlobalPosition = Mathf.Pow(1 - t, 3) * _controlPoints[0] +
								 3 * Mathf.Pow(1 - t, 2) * t * _controlPoints[1] +
								 3 * (1 - t) * t * t * _controlPoints[2] +
								 Mathf.Pow(t, 3) * _controlPoints[3];

			if (i > 0)
			{
				_nodes[i].Rotation = Vector2.Up.AngleTo(_nodes[i].Position - _nodes[i - 1].Position);
			}

			var scale = _scaleFactor * (1f - 0.01f * (_nodes.Count - 1 - i));
			_nodes[i].Scale = new Vector2(scale, scale);
		}

		_nodes[0].Rotation = _nodes[1].Rotation;
	}
}
