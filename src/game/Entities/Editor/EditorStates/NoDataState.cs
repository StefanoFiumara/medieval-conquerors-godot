using MedievalConquerors.Extensions;

namespace MedievalConquerors.Entities.Editor.EditorStates;

public class NoDataState(CardDataEditor editor) : EditorState(editor)
{
    public override void Enter()
    {
        Editor.SetStatus("No Card Data Loaded".Red().Italics());
        Editor.Disable();
    }

    public override void Exit() { }
}
