using MedievalConquerors.Extensions;

namespace MedievalConquerors.Addons.CardDataEditor.UIStates;

public class CreatingNewCardState : CardDataEditorState
{
    public CreatingNewCardState(CardDataEditor editor) : base(editor) { }

    public override void Enter()
    {
        Editor.PanelTitle.Text = "New Card".Orange().Italics().Right();
        
        Editor.SaveButton.Disabled = false;
        Editor.CardTitle.Editable = true;
        Editor.Description.Editable = true;
        Editor.CardType.Disabled = false;

        Editor.AttributeSelector.Disabled = false;
        Editor.AddAttributeButton.Disabled = false;
        Editor.Tags.Enable();

        Editor.IsDirty = true;
    }

    public override void Exit() { }
}