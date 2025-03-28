using MedievalConquerors.Extensions;

namespace MedievalConquerors.Editor.UIStates;

public class EditingExistingCardState : EditorState
{
    public EditingExistingCardState(CardDataEditor editor) : base(editor) { }

    public override void Enter()
    {
        Editor.SetTitle($"Currently Editing: Card ID {Editor.LoadedData.Id}".Green().Italics());
		Editor.Enable();
    }

    public override void Exit() { }
}