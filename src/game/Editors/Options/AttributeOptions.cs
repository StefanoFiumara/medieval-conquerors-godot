using System;
using MedievalConquerors.Editors.CustomEditors.ValueEditors;
using MedievalConquerors.Engine.Data;

namespace MedievalConquerors.Editors.Options;

public partial class AttributeOptions : TypeOptions<CardAttribute>
{
	protected override bool IsValid(Type t) => t is { IsClass: true, IsAbstract: false };
}
