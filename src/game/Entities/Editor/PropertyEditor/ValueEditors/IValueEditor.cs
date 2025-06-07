using System.Reflection;
using Godot;

namespace MedievalConquerors.Entities.Editor.ValueEditors;

public interface IValueEditor
{
    Control GetControl();
    void Load<TOwner>(TOwner owner, PropertyInfo prop);
}
