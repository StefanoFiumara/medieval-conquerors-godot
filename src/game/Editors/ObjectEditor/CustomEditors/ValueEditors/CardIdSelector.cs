using Godot;
using MedievalConquerors.Engine.Data;

namespace MedievalConquerors.Editors.CustomEditors.ValueEditors;

public partial class CardIdSelector : CenterContainer, IValueEditor
{
    private const int IconMaxWidth = 32;
    private const string MissingIcon = "uid://dnofkwp8xw7ys";

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

        _options.AddIconItem(GD.Load<Texture2D>(MissingIcon), "None");
        popup.SetItemIconMaxWidth(0, IconMaxWidth);

        using var database = new CardDatabase();
        var cards = database.Query.OrderBy(c => c.Id)
            .Select(c => new
            {
                c.Id,
                c.Title,
                PortraitUid = c.CardPortraitUid
            }).ToList();

        for(int i = 0; i < cards.Count; i++)
        {
            var card = cards[i];
            var iconPath = ResourceUid.TextToId(card.PortraitUid) == ResourceUid.InvalidId
                ? MissingIcon
                : card.PortraitUid;
            var tex = GD.Load<Texture2D>(iconPath);
            _options.AddIconItem(tex, card.Title, card.Id);
            popup.SetItemIconMaxWidth(i + 1, IconMaxWidth);
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
