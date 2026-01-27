using System;
using MedievalConquerors.Editors.CustomEditors.ValueEditors;
using MedievalConquerors.Engine.Attributes;

namespace MedievalConquerors.Editors.Options;

public partial class AbilityOptions : TypeOptions<AbilityAttribute>
{
    protected override bool IsValid(Type t) => t is { IsClass: true, IsAbstract: false };
}
