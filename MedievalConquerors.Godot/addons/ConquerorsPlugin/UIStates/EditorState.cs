using MedievalConquerors.ConquerorsPlugin.Editor;
using MedievalConquerors.Engine.StateManagement;

namespace MedievalConquerors.ConquerorsPlugin.UIStates;

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
