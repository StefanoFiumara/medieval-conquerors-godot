using System;
using System.Collections.Generic;
using Godot;
using MedievalConquerors.Engine.Actions;
using MedievalConquerors.Engine.Attributes;
using MedievalConquerors.Engine.Data;
using MedievalConquerors.Entities.Editor.Options;

namespace MedievalConquerors.Entities.Editor;

public partial class ActionDefinitionEditor : PanelContainer, IObjectEditor<ActionDefinition>
{
	
	private static PackedScene _paramEditorScene = GD.Load<PackedScene>("uid://bhmiifttu4eln");

	public Control GetControl() => this;

	public void Load(string title, ActionDefinition source, bool allowDelete = false)
	{
		var t = Type.GetType(source.ActionType);
		var parameters = GetParameters(t);

		foreach (var parameter in parameters)
		{
			// TODO: Create an editor for each parameter
		}
		
	}

	private Dictionary<string, Type> GetParameters(Type actionType)
	{
		// TODO: Create a more robust and scalable mapping from Action -> parameters
		// TODO: Is there a way to determine automatically using reflection?
		return actionType switch
		{
			// TODO: Should we use nameof to ensure these don't go out of sync?
			_ when actionType == typeof(DrawCardsAction) =>
				new()
				{
					{ "TargetPlayerId", typeof(PlayerTarget) },
					{ "Amount", typeof(int) }
				},
			_ when actionType == typeof(CreateCardAction) =>
				new()
				{
					{ "TargetPlayerId", typeof(PlayerTarget) },
					{ "CardId", typeof(int) },
					{ "TargetZone", typeof(Zone) },
					{ "Amount", typeof(int) }
					
				},
			_ when actionType == typeof(BuildStructureByIdAction) =>
				new()
				{
					{ "CardId", typeof(int) }
				},
			_ when actionType == typeof(ShuffleDeckAction) => 
				new()
				{
					{ "TargetPlayerId", typeof(PlayerTarget) },
				},
			_ => throw new ArgumentException($"No parameter list defined for {actionType.Name}")
		};
	}

	public ActionDefinition Create()
	{
		// TODO: Serialize parameter editors into comma-delimited list
		throw new System.NotImplementedException();
	}

	public void Enable()
	{
		// TODO: Enable all child parameter editors
		throw new System.NotImplementedException();
	}

	public void Disable()
	{
		// TODO: Disable all child parameter editors
		throw new System.NotImplementedException();
	}
}
