using MedievalConquerors.entities.editor.card_data_editor;
using MedievalConquerors.Extensions;

namespace MedievalConquerors.entities.editor.editor_states;

public class NoDataState(CardDataEditor editor) : EditorState(editor)
{
    public override void Enter()
    {
        Editor.SetTitle("No Card Data Loaded".Red().Italics());
        Editor.Disable();
    }

    public override void Exit() { }
}
