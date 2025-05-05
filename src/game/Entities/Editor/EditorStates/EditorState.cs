using MedievalConquerors.Engine.StateManagement;

namespace MedievalConquerors.Entities.Editor.EditorStates;

public abstract class EditorState(CardDataEditor editor) : IState
{
	protected readonly CardDataEditor Editor = editor;

	public abstract void Enter();
	public abstract void Exit();
}
