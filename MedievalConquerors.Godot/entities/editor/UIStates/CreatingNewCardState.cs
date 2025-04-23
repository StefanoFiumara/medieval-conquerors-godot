using MedievalConquerors.entities.editor.card_data_editor;
using MedievalConquerors.Extensions;

namespace MedievalConquerors.entities.editor.UIStates;

public class CreatingNewCardState(CardDataEditor editor) : EditorState(editor)
{
    public override void Enter()
    {
        Editor.SetTitle("New Card".Orange().Italics());
        Editor.Enable();
    }

    public override void Exit() { }
}
