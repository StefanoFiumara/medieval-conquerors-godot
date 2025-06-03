using MedievalConquerors.Extensions;

namespace MedievalConquerors.Entities.Editor.EditorStates;

public class EditingExistingCardState(CardDataEditor editor) : EditorState(editor)
{
    public override void Enter()
    {
        Editor.SetStatus($"Currently Editing: Card ID {Editor.LoadedData.Id}".Green().Italics());
		Editor.Enable();
    }

    public override void Exit() { }
}
