using MedievalConquerors.Engine.StateManagement;

namespace MedievalConquerors.Addons.CardDataEditor.UIStates;

public abstract class CardDataEditorState : IState
{
    protected readonly Addons.CardDataEditor.CardDataEditor Editor;

    protected CardDataEditorState(Addons.CardDataEditor.CardDataEditor editor)
    {
        Editor = editor;
    }

    public abstract void Enter();
    public abstract void Exit();
}