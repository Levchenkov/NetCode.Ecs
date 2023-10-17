// See https://aka.ms/new-console-template for more information

using NetCode.Ecs;

Console.WriteLine("Hello, World!");

// todo: make resizable
// todo: bench linked vs array ? Some blogger told that it is the same

var world = new World();
world.InitSparseSetFor<Health>();
world.InitSparseSetFor<DamageCollection>();
world.InitSparseSetFor<Dead>();

var systemCollection = new SystemCollection();

systemCollection.Add(new RegenHealthSystem());
systemCollection.Add(new DamageSystem());

systemCollection.Init(world);

systemCollection.Update();

public sealed class RegenHealthSystem : IInitializableSystem, IUpdatableSystem
{
    private SparseSet<Health> _healths;

    public void Init(IWorld world)
    {
        _healths = world.GetSparseSet<Health>();
    }

    public void Update()
    {
        var components = _healths.Components;
        for (int i = 0; i < components.Length; i++)
        {
            ref var health = ref components[i];
            health.Current = Math.Min(health.Current + 1, health.Max);
        }
    }
}

public sealed class DamageSystem : IInitializableSystem, IUpdatableSystem
{
    private SparseSet<Health> _healths;
    private SparseSet<DamageCollection> _damages;
    private SparseSet<Dead> _deaths;

    public void Init(IWorld world)
    {
        _healths = world.GetSparseSet<Health>();
        _damages = world.GetSparseSet<DamageCollection>();
        _deaths = world.GetSparseSet<Dead>();
    }

    public void Update()
    {
        var damageCollectionsComponents = _damages.Components;
        var damageCollectionsEntities = _damages.Entities;

        for (int i = 0; i < damageCollectionsComponents.Length; i++)
        {
            var damageCollection = damageCollectionsComponents[i];
            if (damageCollection.Damages == null || damageCollection.Damages.Length == 0)
            {
                continue;
            }

            var victimId = damageCollectionsEntities[i];
            ref var health = ref _healths.GetComponent(victimId);
            var currentHealth = (float)health.Current;

            foreach (var damage in damageCollection.Damages)
            {
                currentHealth -= damage.Value;

                if (!(currentHealth < 0))
                    continue;

                health.Current = 0;
                ref var dead = ref _deaths.AddComponent(victimId);
                dead.KillerId = damage.DamageDealerId;
            }
        }
    }
}

public struct Health
{
    public int Current;

    public int Max;
}

public struct DamageCollection
{
    public Damage[]? Damages;
}

public struct Damage
{
    public float Value;

    public EntityId DamageDealerId;
}

public struct Dead
{
    public EntityId KillerId;
}