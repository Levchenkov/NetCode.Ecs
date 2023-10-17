namespace NetCode.Ecs;

public class DenseSet<T>
    where T : struct
{
    private T[] _dense;
    private EntityId[] _entities;
    private int _count;

    public int Count => _count;

    public Span<T> Components => new(_dense, 0, _count);

    public Span<EntityId> Entities => new (_entities, 0, _count);

    public DenseSet(int capacity)
    {
        _dense = new T[capacity];
        _entities = new EntityId[capacity];
        _count = 0;
    }

    public ref T GetByIndex(int index)
    {
#if DEBUG
        if (index < 0)
            throw new ArgumentOutOfRangeException(nameof(index), index, "Should be non negative.");

        if (index >= _count)
            throw new ArgumentOutOfRangeException(nameof(index), index, "Should be less than Count");
#endif

        return ref _dense[index];
    }

    public ref T Add(EntityId entityId)
    {
        if (_count == _dense.Length)
            throw new MaxCapacityException(typeof(T));

        _entities[_count] = entityId;
        return ref _dense[_count++];
    }

    public EntityId Delete(int index)
    {
        _dense[index] = _dense[_count - 1];
        _dense[_count - 1] = default;

        _entities[index] = _entities[_count - 1];

        _count--;

        return _entities[_count];
    }
}