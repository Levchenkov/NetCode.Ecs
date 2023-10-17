using System.Runtime.CompilerServices;
using Leopotam.EcsLite;

namespace NetCode.Ecs.Benchmarks;

public class EcsLiteWrapper<T> : IEcsWrapper<T>
    where T : struct
{
    private readonly EcsPool<T> _pool;

    public EcsLiteWrapper(int entityCount)
    {
        var world = new EcsWorld ();
        _pool = world.GetPool<T>();
        for (int i = 0; i < entityCount; i++)
        {
            world.NewEntity();
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ref T Add(int id)
    {
        return ref _pool.Add(id);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ref T Get(int id)
    {
        return ref _pool.Get(id);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Delete(int id)
    {
        _pool.Del(id);
    }
}