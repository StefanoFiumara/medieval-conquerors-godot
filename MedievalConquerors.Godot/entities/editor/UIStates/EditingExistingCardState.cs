using MedievalConquerors.entities.editor.card_data_editor;
using MedievalConquerors.Extensions;

namespace MedievalConquerors.entities.editor.UIStates;

public class EditingExistingCardState(CardDataEditor editor) : EditorState(editor)
{
    public override void Enter()
    {
        Editor.SetTitle($"Currently Editing: Card ID {Editor.LoadedData.Id}".Green().Italics());
		Editor.Enable();
    }

    public override void Exit() { }
}
