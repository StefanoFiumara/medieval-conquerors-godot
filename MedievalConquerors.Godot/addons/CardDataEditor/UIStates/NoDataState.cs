using MedievalConquerors.Extensions;

namespace MedievalConquerors.Addons.CardDataEditor.UIStates;

public class NoDataState : CardDataEditorState
{
    public NoDataState(Addons.CardDataEditor.CardDataEditor editor) : base(editor) { }

    public override void Enter()
    {
        Editor.PanelTitle.Text = "No Card Data Loaded".Red().Italics().Right();

        // TODO: Highlight New Button somehow?
        Editor.SaveButton.Disabled = true;
        Editor.CardTitle.Editable = false;
        Editor.Description.Editable = false;
        Editor.CardType.Disabled = true;
        Editor.Tags.Disable();
		
        Editor.IsDirty = false;
    }

    public override void Exit() { }
}