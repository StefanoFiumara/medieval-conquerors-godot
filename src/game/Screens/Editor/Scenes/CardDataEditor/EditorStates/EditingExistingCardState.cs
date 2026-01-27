using MedievalConquerors.Engine.Extensions;

namespace MedievalConquerors.Screens.Editor.Scenes.CardDataEditor.EditorStates;

public class EditingExistingCardState(CardDataEditor editor) : EditorState(editor)
{
    public override void Enter()
    {
        Editor.SetStatus($"Currently Editing: Card ID {Editor.CurrentCardId}".Green().Italics());
		Editor.Enable();
    }

    public override void Exit() { }
}
