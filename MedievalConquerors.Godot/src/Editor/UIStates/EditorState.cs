using MedievalConquerors.Engine.StateManagement;

namespace MedievalConquerors.Editor.UIStates;

public abstract class EditorState : IState
{
	protected readonly CardDataEditor Editor;

	protected EditorState(CardDataEditor editor)
	{
		Editor = editor;
	}

	public abstract void Enter();
	public abstract void Exit();
}
