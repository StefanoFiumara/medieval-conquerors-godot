using Godot;
using MedievalConquerors.Engine.Data;

namespace MedievalConquerors.Engine.Actions;

public class PlayActionCardAction(Card card, Vector2I targetTile) : GameAction
{
    public Card Card { get; } = card;
    public Vector2I TargetTile { get; } = targetTile;
}