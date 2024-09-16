using System.Collections.Generic;
using System.Linq;

namespace MedievalConquerors.Engine.Core;

public interface IGame : IAwake, IUpdate, IDestroy 
{
	T AddComponent<T> (string key = null) where T : IGameComponent, new ();
	T AddComponent<T> (T component, string key = null) where T : IGameComponent;
	T GetComponent<T> (string key = null) where T : IGameComponent;
	ICollection<IGameComponent> Components { get; }
}

public class Game : IGame
{
	private readonly Dictionary<string, IGameComponent> _components = new();
	public ICollection<IGameComponent> Components => _components.Values;
	private List<IUpdate> _updateable = new();
	
	public T AddComponent<T> (string key = null) where T : IGameComponent, new() => AddComponent(new T(), key);
	
	public TComponent AddComponent<TComponent> (TComponent component, string key = null) 
		where TComponent : IGameComponent
	{
		key ??= typeof(TComponent).Name;

		_components.Add(key, component);
		component.Game = this;
		
		if(component is IUpdate u) _updateable.Add(u);
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

	public void Awake()
	{
		foreach (var system in Components.OfType<IAwake>())
		{
			system.Awake();
		}
	}
	
	public void Update()
	{
		foreach (var system in _updateable)
		{
			system.Update();
		}
	}
	
	public void Destroy()
	{
		foreach (var system in Components.OfType<IDestroy>())
		{
			system.Destroy();
		}
	}	
}
