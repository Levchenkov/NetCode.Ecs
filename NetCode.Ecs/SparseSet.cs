using System.Runtime.CompilerServices;

namespace NetCode.Ecs;

public class SparseSet<T>
    where T : struct
{
    private const int InvalidIndex = -1;

    private readonly int[] _indexes;
    private readonly DenseSet<T> _denseSet;

    public Span<T> Components
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _denseSet.Components;
    }

    public Span<EntityId> Entities
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _denseSet.Entities;
    }

    public SparseSet(int maxEntitiesCount, int maxComponentsPerSet)
    {
        _indexes = new int[maxEntitiesCount];
        _denseSet = new DenseSet<T>(maxComponentsPerSet);

        for (int i = 0; i < maxEntitiesCount; i++)
        {
            _indexes[i] = InvalidIndex;
        }
    }

    public bool HasComponent(EntityId entityId)
    {
        return _indexes[entityId.Id] != InvalidIndex;
    }

    public ref T GetComponent(EntityId entityId)
    {
        var index = _indexes[entityId.Id];

#if DEBUG
        if (index == InvalidIndex)
            ThrowComponentNotFoundException(entityId);
#endif

        return ref _denseSet.GetByIndex(index);
    }

    public ref T AddComponent(EntityId entityId)
    {
        ref var index = ref _indexes[entityId.Id];

        if (index != InvalidIndex)
            return ref _denseSet.GetByIndex(index);

        ref var component = ref _denseSet.Add(entityId);
        index = _denseSet.Count - 1;

        return ref component;
    }

    public void DeleteComponent(EntityId entityId)
    {
        var index = _indexes[entityId.Id];

        if (index == InvalidIndex)
            return;

        var replacedEntityId = _denseSet.Delete(index);

        _indexes[replacedEntityId.Id] = index;

        _indexes[entityId.Id] = InvalidIndex;
    }

    private static void ThrowComponentNotFoundException(EntityId entityId)
    {
        throw new ComponentNotFoundException(entityId);
    }
}