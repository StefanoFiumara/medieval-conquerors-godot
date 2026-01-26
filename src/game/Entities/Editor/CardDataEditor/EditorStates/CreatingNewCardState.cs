using MedievalConquerors.Extensions;

namespace MedievalConquerors.Entities.Editor.EditorStates;

public class CreatingNewCardState(CardDataEditor editor) : EditorState(editor)
{
    public override void Enter()
    {
        Editor.SetStatus("New Card".Orange().Italics());
        Editor.Enable();
    }

    public override void Exit() { }
}
