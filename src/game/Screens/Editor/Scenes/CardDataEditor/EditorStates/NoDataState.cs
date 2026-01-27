using MedievalConquerors.Engine.Extensions;

namespace MedievalConquerors.Screens.Editor.Scenes.CardDataEditor.EditorStates;

public class NoDataState(CardDataEditor editor) : EditorState(editor)
{
    public override void Enter()
    {
        Editor.SetStatus("No Card Data Loaded".Red().Italics());
        Editor.Disable();
    }

    public override void Exit() { }
}
