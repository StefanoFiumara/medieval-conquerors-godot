using MedievalConquerors.ConquerorsPlugin.Editor;
using MedievalConquerors.Extensions;

namespace MedievalConquerors.ConquerorsPlugin.UIStates;

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