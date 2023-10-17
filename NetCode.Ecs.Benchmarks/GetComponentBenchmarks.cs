using BenchmarkDotNet.Attributes;

namespace NetCode.Ecs.Benchmarks;

/// <summary>
/// BenchmarkDotNet v0.13.9+228a464e8be6c580ad9408e98f18813f6407fb5a, Windows 11 (10.0.22621.2283/22H2/2022Update/SunValley2)
/// AMD Ryzen 7 5800H with Radeon Graphics, 1 CPU, 16 logical and 8 physical cores
/// .NET SDK 7.0.306
///   [Host]     : .NET 7.0.9 (7.0.923.32018), X64 RyuJIT AVX2
///   DefaultJob : .NET 7.0.9 (7.0.923.32018), X64 RyuJIT AVX2
///
///
/// | Method                | Mean      | Error    | StdDev   | Allocated |
/// |---------------------- |----------:|---------:|---------:|----------:|
/// | GetComponentSparseSet |  67.60 ns | 0.223 ns | 0.208 ns |         - |
/// | GetComponentArray     |  49.34 ns | 0.067 ns | 0.062 ns |         - |
/// </summary>
// [Config(typeof(AntiVirusFriendlyConfig))]
[MemoryDiagnoser]
public class GetComponentBenchmarks
{
    private IWorld _world;
    private SparseSet<TestComponent> _sparseSet;

    private TestComponent[] _array;

    private int _count = 100;

    public GetComponentBenchmarks()
    {
        var world = new World();
        world.InitSparseSetFor<TestComponent>();

        _world = world;
        _sparseSet = _world.GetSparseSet<TestComponent>();

        for (ushort i = 1; i <= _count; i++)
        {
            _sparseSet.AddComponent(new EntityId(i));
        }

        _array = new TestComponent[_count + 1];
    }

    [Benchmark]
    public void GetComponentSparseSet()
    {
        for (ushort i = 1; i <= 100; i++)
        {
            ref var c = ref _sparseSet.GetComponent(new EntityId(i));
        }
    }

    [Benchmark]
    public void GetComponentArray()
    {
        for (ushort i = 1; i <= 100; i++)
        {
            ref var c = ref _array[new EntityId(i).Id];
        }
    }
}