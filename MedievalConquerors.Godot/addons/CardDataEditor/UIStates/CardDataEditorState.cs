using MedievalConquerors.Engine.StateManagement;

namespace MedievalConquerors.Addons.CardDataEditor.UIStates;

public abstract class CardDataEditorState : IState
{
    protected readonly CardDataEditor Editor;

    protected CardDataEditorState(CardDataEditor editor)
    {
        Editor = editor;
    }

    public abstract void Enter();
    public abstract void Exit();
}