using MedievalConquerors.Extensions;

namespace MedievalConquerors.Addons.CardDataEditor.UIStates;

public class NoDataState : EditorState
{
    public NoDataState(CardDataEditor editor) : base(editor) { }

    public override void Enter()
    {
        Editor.SetTitle("No Card Data Loaded".Red().Italics());
        Editor.Disable();
    }

    public override void Exit() { }
}