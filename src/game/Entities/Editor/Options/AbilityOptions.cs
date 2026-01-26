using System;
using MedievalConquerors.Engine.Attributes;
using MedievalConquerors.Entities.Editor.ValueEditors;

namespace MedievalConquerors.Entities.Editor.Options;

public partial class AbilityOptions : TypeOptions<AbilityAttribute>
{
    protected override bool IsValid(Type t) => t is { IsClass: true, IsAbstract: false };
}
