using System;

namespace MedievalConquerors.Editors;

[AttributeUsage(AttributeTargets.Property)]
public class UseValueEditorAttribute : Attribute
{
    public Type EditorType { get; }

    public UseValueEditorAttribute(Type editorType)
    {
        if (!typeof(IValueEditor).IsAssignableFrom(editorType))
            throw new ArgumentException($"Editor type must implement IValueEditor: {editorType}");

        EditorType = editorType;
    }
}
