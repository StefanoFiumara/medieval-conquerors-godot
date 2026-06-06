using Godot;

namespace MedievalConquerors.Engine.Utils;

public partial class Timer(double length) : Node
{
    public double Length { get; } = length;
    public bool IsElapsed => _time > Length;
    public bool IsRunning { get; private set; }

    private double _time = 0;

    public void Start() => IsRunning = true;
    public void Reset() => _time = 0;
    public void Stop()
    {
        IsRunning = false;
        Reset();
    }

    public override void _PhysicsProcess(double delta)
    {
        if(!IsRunning) return;
        _time += delta;
    }
}
