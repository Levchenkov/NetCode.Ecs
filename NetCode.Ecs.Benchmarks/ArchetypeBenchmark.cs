using System.Runtime.CompilerServices;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Diagnosers;

namespace NetCode.Ecs.Benchmarks;

/// <summary>
/// BenchmarkDotNet v0.13.9+228a464e8be6c580ad9408e98f18813f6407fb5a, Windows 11 (10.0.22621.2428/22H2/2022Update/SunValley2)
/// 12th Gen Intel Core i7-12700, 1 CPU, 20 logical and 12 physical cores
/// .NET SDK 7.0.202
///   [Host]     : .NET 7.0.4 (7.0.423.11508), X64 RyuJIT AVX2
///   DefaultJob : .NET 7.0.4 (7.0.423.11508), X64 RyuJIT AVX2
///
///
/// | Method                      | Mean      | Error    | StdDev   | Ratio | Allocated | Alloc Ratio |
/// |-----------------------------|----------:|---------:|---------:|------:|----------:|------------:|
/// | SingleComponent             |  59.85 ns | 0.566 ns | 0.529 ns |  0.94 |         - |          NA |
/// | ArrayOfStructs              |  58.59 ns | 0.165 ns | 0.154 ns |  1.00 |         - |          NA |
/// | LinkedList                  | 136.33 ns | 1.668 ns | 1.560 ns |  2.31 |         - |          NA |
/// | Arrays                      |  65.66 ns | 0.285 ns | 0.252 ns |  1.13 |         - |          NA |
/// | StructWithArrays            | 108.51 ns | 0.533 ns | 0.498 ns |  1.85 |         - |          NA |
/// | RefStruct                   |  70.38 ns | 0.573 ns | 0.536 ns |  1.11 |         - |          NA |
/// | UnsafeStruct                |  70.24 ns | 0.956 ns | 0.894 ns |  1.11 |         - |          NA |
/// | PlayerArchetypeSetBenchmark | 128.20 ns | 0.620 ns | 0.580 ns |
/// | ThreeSparseSets             | 199.20 ns | 0.640 ns | 0.540 ns |

/// </summary>
// [MemoryDiagnoser]
// [HardwareCounters(HardwareCounter.CacheMisses)]
public class ArchetypeBenchmark
{
    private const int Count = 100;

    private readonly LinkedList<PlayerArchetype> _archetypesAsLinkedList;
    private readonly PlayerArchetype[] _archetypes;
    private readonly Position[] _positions;
    private readonly Health[] _healths;
    private readonly Speed[] _speeds;

    private readonly PlayerArchetypeWithArrays[] _archetypes2;
    private readonly PlayerArchetypeSet _playerArchetypeSet;
    private readonly SparseSet<Position> _positionSet;
    private readonly SparseSet<Health> _healthSet;
    private readonly SparseSet<Speed> _speedSet;

    public ArchetypeBenchmark()
    {
        _archetypes = new PlayerArchetype[Count];
        _positions = new Position[Count];
        _healths = new Health[Count];
        _speeds = new Speed[Count];
        _archetypes2 = new PlayerArchetypeWithArrays[Count];

        _playerArchetypeSet = new PlayerArchetypeSet(Count, Count, _positions, _healths, _speeds);
        _positionSet = new SparseSet<Position>(Count, Count);
        _healthSet = new SparseSet<Health>(Count, Count);
        _speedSet = new SparseSet<Speed>(Count, Count);

        for (int i = 0; i < Count; i++)
        {
            var entityId = (EntityId)i;

            _playerArchetypeSet.AddEntity(entityId, i, i, i);
            _positionSet.AddComponent(entityId);
            _healthSet.AddComponent(entityId);
            _speedSet.AddComponent(entityId);
        }

        for (int i = 0; i < Count; i++)
        {
            _archetypes2[i] = new PlayerArchetypeWithArrays(_positions, i, _healths, i, _speeds, i);
        }

        _archetypesAsLinkedList = new LinkedList<PlayerArchetype>();
        for (int i = 0; i < Count; i++)
        {
            _archetypesAsLinkedList.AddLast(new PlayerArchetype());
        }
    }

    // [Benchmark]
    public int SingleComponent()
    {
        var s = 0;
        for (int i = 0; i < Count; i++)
        {
            ref var position = ref _positions[i];
            s += position.X + position.Y + position.Z;
        }

        return s;
    }

    // [Benchmark(Baseline = true)]
    public int ArrayOfStructs()
    {
        var s = 0;
        for (int i = 0; i < Count; i++)
        {
            ref var player = ref _archetypes[i];
            s += player.Health.Current + player.Position.X + player.Speed.X;
        }

        return s;
    }

    // [Benchmark]
    public int LinkedList()
    {
        var s = 0;
        var playerNode = _archetypesAsLinkedList.First;
        for (int i = 0; i < Count; i++)
        {
            ref var player = ref playerNode.ValueRef;
            s += player.Health.Current + player.Position.X + player.Speed.X;
            playerNode = playerNode.Next;
        }

        return s;
    }

    // [Benchmark]
    public int Arrays()
    {
        var s = 0;
        for (int i = 0; i < Count; i++)
        {
            s += _healths[i].Current + _positions[i].X + _speeds[i].X;
        }

        return s;
    }

    // [Benchmark]
    public int StructWithArrays()
    {
        var s = 0;
        for (int i = 0; i < Count; i++)
        {
            ref var player = ref _archetypes2[i];
            s += player.Health.Current + player.Position.X + player.Speed.X;
        }

        return s;
    }

    // [Benchmark]
    public int RefStruct()
    {
        var s = 0;

        for (int i = 0; i < Count; i++)
        {
            var player = GetPlayer(i);

            s += player.Health.Current + player.Position.X + player.Speed.X;
        }

        return s;
    }

    [Benchmark]
    public int PlayerArchetypeSetBenchmark()
    {
        var s = 0;

        for (int i = 0; i < Count; i++)
        {
            var player = _playerArchetypeSet.Get((EntityId)i);

            s += player.Health.Current + player.Position.X + player.Speed.X;
        }

        return s;
    }

    [Benchmark]
    public int ThreeSparseSets()
    {
        var s = 0;

        for (int i = 0; i < Count; i++)
        {
            var position = _positionSet.GetComponent((EntityId)i);
            var health = _healthSet.GetComponent((EntityId)i);
            var speed = _speedSet.GetComponent((EntityId)i);

            s += health.Current + position.X + speed.X;
        }

        return s;
    }

    // [Benchmark]
    public int UnsafeStruct()
    {
        var s = 0;

        for (int i = 0; i < Count; i++)
        {
            var player = GetPlayerUnsafe(i);

            s += player.Health.Current + player.Position.X + player.Speed.X;
        }

        return s;
    }

    private PlayerArchetypeRefStruct GetPlayer(int entityId)
    {
        return new PlayerArchetypeRefStruct(ref _positions[entityId], ref _healths[entityId], ref _speeds[entityId]);
    }

    private PlayerArchetypeUnsafe GetPlayerUnsafe(int entityId)
    {
        return new PlayerArchetypeUnsafe(ref _positions[entityId], ref _healths[entityId], ref _speeds[entityId]);
    }

    public readonly unsafe ref struct PlayerArchetypeUnsafe
    {
        private readonly void* _positionPtr;
        private readonly void* _healthPtr;
        private readonly void* _speedPtr;

        public PlayerArchetypeUnsafe(ref Position componentRef, ref Health componentRef2, ref Speed componentRef3)
        {
            _positionPtr = Unsafe.AsPointer(ref componentRef);
            _healthPtr = Unsafe.AsPointer(ref componentRef2);
            _speedPtr = Unsafe.AsPointer(ref componentRef3);
        }

        public ref Position Position
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ref Unsafe.AsRef<Position>(_positionPtr);
        }

        public ref Health Health
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ref Unsafe.AsRef<Health>(_healthPtr);
        }

        public ref Speed Speed
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ref Unsafe.AsRef<Speed>(_speedPtr);
        }
    }

    public ref struct PlayerArchetypeRefStruct
    {
        public PlayerArchetypeRefStruct(ref Position position, ref Health health, ref Speed speed)
        {
            Position = ref position;
            Health = ref health;
            Speed = ref speed;
        }

        public ref Position Position;
        public ref Health Health;
        public ref Speed Speed;
    }

    public struct PlayerArchetypeWithArrays
    {
        private Position[] _positions;
        private int _positionIndex;

        private Health[] _healths;
        private int _healthIndex;

        private Speed[] _speeds;
        private int _speedIndex;

        public PlayerArchetypeWithArrays(Position[] positions, int positionIndex, Health[] healths, int healthIndex, Speed[] speeds, int speedIndex)
        {
            _positions = positions;
            _positionIndex = positionIndex;
            _healths = healths;
            _healthIndex = healthIndex;
            _speeds = speeds;
            _speedIndex = speedIndex;
        }

        public ref Position Position => ref _positions[_positionIndex];
        public ref Health Health => ref _healths[_healthIndex];
        public ref Speed Speed => ref _speeds[_speedIndex];
    }

    public struct PlayerArchetype
    {
        public Position Position;
        public Health Health;
        public Speed Speed;
    }

    public struct Position
    {
        public int X;
        public int Y;
        public int Z;
    }

    public struct Health
    {
        public int Current;
        public int Max;
    }

    public struct Speed
    {
        public int X;
        public int Y;
    }

    public struct PlayerArchetypeData
    {
        public int PositionIndex;
        public int HealthIndex;
        public int SpeedIndex;
        public bool IsValid;
    }

    // todo: public class PlayerArchetypeSet<TA, TAD, TC1, TC2, TC3>?
    public sealed class PlayerArchetypeSet
    {
        private const int InvalidIndex = -1;

        private readonly Position[] _positions;
        private readonly Health[] _healths;
        private readonly Speed[] _speeds;

        private readonly PlayerArchetypeData[] _indexes;
        private readonly EntityId[] _entities;
        private int _count;

        public PlayerArchetypeSet(int maxEntitiesCount, int maxEntitiesPerArchetype, Position[] positionSet, Health[] healthSet, Speed[] speedSet)
        {
            _positions = positionSet;
            _healths = healthSet;
            _speeds = speedSet;
            _indexes = new PlayerArchetypeData[maxEntitiesCount];
            _entities = new EntityId[maxEntitiesPerArchetype];

        }

        public bool Has(EntityId entityId)
        {
            return _indexes[entityId.Id].IsValid;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public PlayerArchetypeRefStruct Get(EntityId entityId)
        {
            var data = _indexes[entityId.Id];
#if DEBUG
            if (!data.IsValid)
                throw new Exception("Archetype is not valid");
#endif
            return new PlayerArchetypeRefStruct(
                ref _positions[data.PositionIndex],
                ref _healths[data.HealthIndex],
                ref _speeds[data.SpeedIndex]);
        }

        public Span<EntityId> Entities => new(_entities, 0, _count);

        public void AddEntity(EntityId entityId, int positionIndex, int healthIndex, int speedIndex)
        {
            ref var data = ref _indexes[entityId.Id];
            if (data.IsValid)
                return;

            data.IsValid = true;
            data.PositionIndex = positionIndex;
            data.HealthIndex = healthIndex;
            data.SpeedIndex = speedIndex;

            _count++;
        }

        public void DeleteEntity(EntityId entityId)
        {
            ref var data = ref _indexes[entityId.Id];
            if (!data.IsValid)
                return;

            data.IsValid = false;
            _count--;
        }
    }
}