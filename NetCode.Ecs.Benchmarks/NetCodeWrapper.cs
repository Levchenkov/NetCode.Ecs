using System.Runtime.CompilerServices;

namespace NetCode.Ecs.Benchmarks;

public class NetCodeWrapper<T> : IEcsWrapper<T>
    where T : struct
{
    private readonly SparseSet<T> _sparseSet;

    public NetCodeWrapper(int entityCount)
    {
        var world = new World();
        world.InitSparseSetFor<T>(entityCount, entityCount);

        _sparseSet = world.GetSparseSet<T>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ref T Add(int id)
    {
        return ref _sparseSet.AddComponent(new EntityId((ushort)id));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ref T Get(int id)
    {
        return ref _sparseSet.GetComponent(new EntityId((ushort)id));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Delete(int id)
    {
        _sparseSet.DeleteComponent(new EntityId((ushort)id));
    }
}