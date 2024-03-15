using MedievalConquerors.Extensions;

namespace MedievalConquerors.Addons.CardDataEditor.UIStates;

public class NoDataState : CardDataEditorState
{
    public NoDataState(CardDataEditor editor) : base(editor) { }

    public override void Enter()
    {
        Editor.PanelTitle.Text = "No Card Data Loaded".Red().Italics().Right();
        
        Editor.SaveButton.Disabled = true;
        Editor.CardTitle.Editable = false;
        Editor.Description.Editable = false;
        Editor.CardTypeOptions.Disabled = true;
        Editor.TagOptions.Disable();
		
        Editor.IsDirty = false;
        Editor.AttributeSelector.Disabled = true;
        Editor.AddAttributeButton.Disabled = true;
    }

    public override void Exit() { }
}