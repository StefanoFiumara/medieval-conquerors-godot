using Godot;

namespace MedievalConquerors.Entities.Editor.ValueEditors;

public interface IValueEditor
{
	Control GetControl();
	object GetValue();
}
