using System;
using MedievalConquerors.Editors.CustomEditors.ValueEditors;
using MedievalConquerors.Engine.Attributes;

namespace MedievalConquerors.Editors.Options;

public partial class TargetSelectorOptions : TypeOptions<TargetSelector>
{
    protected override bool IsValid(Type t) => !t.IsAbstract;
}