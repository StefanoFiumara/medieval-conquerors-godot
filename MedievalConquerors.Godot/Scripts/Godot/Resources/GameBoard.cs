using System.Collections.Generic;
using Godot;
using MedievalConquerors.Engine.Data;

namespace MedievalConquerors.Godot.Resources;

[GlobalClass]
public partial class GameBoard : Resource, IGameBoard
{
    // TODO: Figure out if the Hex Map logic can be abstracted away from Godot's tile system, so we can move it to the engine
    public ITile GetTile(int x, int y, int z)
    {
        throw new System.NotImplementedException();
    }

    public IEnumerable<ITile> GetNeighbors(int x, int y, int z)
    {
        throw new System.NotImplementedException();
    }

    public IEnumerable<ITile> GetReachable(int x, int y, int z, int range)
    {
        throw new System.NotImplementedException();
    }

    public string GetMapStateHash()
    {
        throw new System.NotImplementedException();
    }
}