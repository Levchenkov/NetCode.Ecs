using BenchmarkDotNet.Attributes;

namespace NetCode.Ecs.Benchmarks;

/// <summary>
/// BenchmarkDotNet v0.13.9+228a464e8be6c580ad9408e98f18813f6407fb5a, Windows 11 (10.0.22621.2428/22H2/2022Update/SunValley2)
/// AMD Ryzen 7 5800H with Radeon Graphics, 1 CPU, 16 logical and 8 physical cores
/// .NET SDK 7.0.306
///   [Host]     : .NET 7.0.9 (7.0.923.32018), X64 RyuJIT AVX2
///   DefaultJob : .NET 7.0.9 (7.0.923.32018), X64 RyuJIT AVX2
///
///
/// | Method     | Mean     | Error     | StdDev    | Allocated |
/// |----------- |---------:|----------:|----------:|----------:|
/// | NetCodeEcs | 2.799 us | 0.0047 us | 0.0042 us |         - |
/// | LeoEcsLite | 3.145 us | 0.0018 us | 0.0015 us |         - |
/// </summary>
// [Config(typeof(AntiVirusFriendlyConfig))]
[MemoryDiagnoser]
public class CompareBenchmark
{
    private int TickCount = 100;

    private IEcsWrapper<TestComponent> _netCodeWrapper;
    private IEcsWrapper<TestComponent> _ecsliteWrapper;

    public CompareBenchmark()
    {
        _netCodeWrapper = new NetCodeWrapper<TestComponent>(100);
        _ecsliteWrapper = new EcsLiteWrapper<TestComponent>(100);
    }

    [Benchmark]
    public void NetCodeEcs()
    {
        ExecuteFlow(_netCodeWrapper);
    }

    // [Benchmark]
    public void NetCodeEcs_AddOnly()
    {
        for (int i = 0; i < TickCount; i++)
        {
            _netCodeWrapper.Add(i);
        }
    }

    // [Benchmark]
    // public void NetCodeEcs_GetOnly()
    // {
    //
    // }
    //
    // [Benchmark]
    // public void NetCodeEcs_DeleteOnly()
    // {
    //
    // }

    [Benchmark]
    public void LeoEcsLite()
    {
        ExecuteFlow(_ecsliteWrapper);
    }

    // not working
    // [Benchmark]
    // public void LeoEcsLite_AddOnly()
    // {
    //     for (int i = 0; i < TickCount; i++)
    //     {
    //         _ecsliteWrapper.Add(i);
    //     }
    // }

    private void ExecuteFlow<T>(IEcsWrapper<T> wrapper)
    {
        var minId = 1;
        var maxId = 1;

        for (int tick = 0; tick < TickCount; tick++)
        {
            if (tick % 2 == 0)
            {
                wrapper.Add(maxId);
                maxId++;
            }

            for (int i = minId; i < maxId; i++)
            {
                ref var c = ref wrapper.Get(i);
            }

            if (tick % 5 == 0)
            {
                wrapper.Delete(minId);
                minId++;
            }
        }
    }
}