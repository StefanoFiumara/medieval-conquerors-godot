using MedievalConquerors.Engine.StateManagement;
using MedievalConquerors.entities.editor.card_data_editor;

namespace MedievalConquerors.entities.editor.editor_states;

public abstract class EditorState(CardDataEditor editor) : IState
{
	protected readonly CardDataEditor Editor = editor;

	public abstract void Enter();
	public abstract void Exit();
}
