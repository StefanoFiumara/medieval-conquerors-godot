using System.Collections.Generic;

namespace MedievalConquerors.Engine.Core;

public interface IGame 
{
	T AddComponent<T> (string key = null) where T : IGameComponent, new ();
	T AddComponent<T> (T component, string key = null) where T : IGameComponent;
	T GetComponent<T> (string key = null) where T : IGameComponent;
	ICollection<IGameComponent> Components();
}

public class Game : IGame
{
	private readonly Dictionary<string, IGameComponent> _components = new();
	public ICollection<IGameComponent> Components() => _components.Values;
	
	public T AddComponent<T> (string key = null) where T : IGameComponent, new() => AddComponent(new T(), key);
	
	public TComponent AddComponent<TComponent> (TComponent component, string key = null) 
		where TComponent : IGameComponent
	{
		key ??= typeof(TComponent).Name;

		_components.Add(key, component);
		component.Game = this;
		return component;
	}

	public TComponent GetComponent<TComponent> (string key = null) 
		where TComponent : IGameComponent
	{
		key ??= typeof(TComponent).Name;

		return _components.TryGetValue(key, out var component) 
			? (TComponent)component 
			: default;
	}
}
