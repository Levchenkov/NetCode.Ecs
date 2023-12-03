using System.Numerics;

namespace NetCode.Ecs.UnitTests;

public struct EmptyComponent
{
}

public struct HealthComponent
{
    public int Current;
    public int Max;
}