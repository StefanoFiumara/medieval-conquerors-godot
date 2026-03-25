using MedievalConquerors.Editors;
using MedievalConquerors.Editors.Options;

namespace MedievalConquerors.Engine.Actions;

public class PassiveResourceCollectionAction(int playerId) : GameAction
{
    public int PlayerId { get; init; } = playerId;

    // TODO: May need to track each collected resource for animation purposes.
}
