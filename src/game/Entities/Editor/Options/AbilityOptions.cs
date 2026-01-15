using System;
using MedievalConquerors.Engine.Attributes;

namespace MedievalConquerors.Entities.Editor.Options;

public partial class AbilityOptions : TypeOptions<AbilityAttribute>
{
    protected override bool IsValid(Type t)
    {
        return t is { IsClass: true, IsAbstract: false };
    }
}