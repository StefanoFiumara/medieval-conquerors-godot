using System.Linq;

namespace MedievalConquerors.Engine.Core;

public interface IAwake
{
	void Awake();
}

public interface IUpdate
{
	void Update();
}

public interface IDestroy
{
	void Destroy();
}

public static class LifecycleExtensions
{
	public static void Awake(this IGame game)
	{
		foreach (var system in game.Components().OfType<IAwake>())
		{
			system.Awake();
		}
	}
	
	public static void Update(this IGame game)
	{
		foreach (var system in game.Components().OfType<IUpdate>())
		{
			system.Update();
		}
	}
	
	public static void Destroy(this IGame game)
	{
		foreach (var system in game.Components().OfType<IDestroy>())
		{
			system.Destroy();
		}
	}	
}
