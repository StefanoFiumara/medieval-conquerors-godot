using Godot;
using MedievalConquerors.Engine.Data;

namespace MedievalConquerors.Editors.CustomEditors.ValueEditors;

public partial class CardIdSelector : CenterContainer, IValueEditor
{
    public Control GetControl() => this;

    // TODO: May want to turn this into CardOptions button later, if we need it in multiple places.
    private OptionButton _options;

    public override void _Ready()
    {
        _options = new OptionButton()
        {
            ExpandIcon = true
        };
        var popup = _options.GetPopup();

        using var database = new CardDatabase();
        var cards = database.Query.OrderBy(c => c.Id).Select(c => new { c.Id, c.Title, c.ImagePath }).ToList();

        for(int i = 0; i < cards.Count; i++)
        {
            var card = cards[i];
            var iconPath = ResourceUid.TextToId(card.ImagePath) == ResourceUid.InvalidId
                ? "uid://dnofkwp8xw7ys" // Missing Icon
                : card.ImagePath;
            var tex = GD.Load<Texture2D>(iconPath);
            _options.AddIconItem(tex, card.Title, card.Id);
            popup.SetItemIconMaxWidth(i, 32);
        }

        AddChild(_options);
    }

    public void Enable() => _options.Disabled = false;
    public void Disable() => _options.Disabled = true;

    public object GetValue() => _options.GetSelectedId();

    public void SetValue(object value)
    {
        if(value is int cardId)
            _options.Selected = _options.GetItemIndex(cardId);
    }
}
