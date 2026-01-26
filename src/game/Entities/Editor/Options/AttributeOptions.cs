using System;
using MedievalConquerors.Engine.Actions;
using MedievalConquerors.Engine.Data;
using MedievalConquerors.Entities.Editor.ValueEditors;

namespace MedievalConquerors.Entities.Editor.Options;

public partial class AttributeOptions : TypeOptions<CardAttribute>
{
	protected override bool IsValid(Type t) => t is { IsClass: true, IsAbstract: false };
}
