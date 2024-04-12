using MedievalConquerors.Extensions;

namespace MedievalConquerors.Addons.CardDataEditor.UIStates;

public class CreatingNewCardState : EditorState
{
    public CreatingNewCardState(CardDataEditor editor) : base(editor) { }

    public override void Enter()
    {
        Editor.SetTitle("New Card".Orange().Italics().Right());
        Editor.Enable();
    }

    public override void Exit() { }
}