using MedievalConquerors.Extensions;

namespace MedievalConquerors.Addons.CardDataEditor.UIStates;

public class EditingExistingCardState : CardDataEditorState
{
    public EditingExistingCardState(Addons.CardDataEditor.CardDataEditor editor) : base(editor) { }

    public override void Enter()
    {
        Editor.PanelTitle.Text = $"Currently Editing: Card Id {Editor.LoadedData.Id}".Green().Italics().Right();
		
        // TODO: Highlight Save Button
        Editor.SaveButton.Disabled = false;
        Editor.CardTitle.Editable = true;
        Editor.Description.Editable = true;
        Editor.CardType.Disabled = false;
        Editor.Tags.Enable();
        
        Editor.IsDirty = false;
    }

    public override void Exit() { }
}