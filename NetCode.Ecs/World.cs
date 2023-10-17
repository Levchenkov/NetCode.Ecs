using System.Runtime.CompilerServices;

namespace NetCode.Ecs;

public class World : IWorld
{
    public const int MaxEntitiesCount = ushort.MaxValue;

    public const int MaxComponentsPerSet = ushort.MaxValue;
    
    private readonly Dictionary<Type, object> _sets;

    public World()
    {
        _sets = new Dictionary<Type, object>();
    }

    public void InitSparseSetFor<T>() where T : struct => InitSparseSetFor<T>(MaxEntitiesCount, MaxComponentsPerSet);
    
    public void InitSparseSetFor<T>(int maxEntitiesCount, int maxComponentsPerSet)
        where T : struct
    {
        var key = typeof(T);

        _sets[key] = new SparseSet<T>(maxEntitiesCount, maxComponentsPerSet);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SparseSet<T> GetSparseSet<T>()
        where T : struct
    {
        var key = typeof(T);

        var set = (SparseSet<T>)_sets[key];

        return set;
    }
}