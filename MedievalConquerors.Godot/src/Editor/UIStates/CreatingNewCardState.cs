using MedievalConquerors.Extensions;

namespace MedievalConquerors.Editor.UIStates;

public class CreatingNewCardState : EditorState
{
    public CreatingNewCardState(CardDataEditor editor) : base(editor) { }

    public override void Enter()
    {
        Editor.SetTitle("New Card".Orange().Italics());
        Editor.Enable();
    }

    public override void Exit() { }
}