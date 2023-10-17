namespace NetCode.Ecs;

public class SystemCollection
{
    private List<IInitializableSystem> _initializableSystems = new List<IInitializableSystem>();

    private List<IUpdatableSystem> _updatableSystems = new List<IUpdatableSystem>();

    public void Add<T>(T system)
    {
        if (system is IInitializableSystem initializableSystem)
        {
            _initializableSystems.Add(initializableSystem);
        }

        if (system is IUpdatableSystem updatableSystem)
        {
            _updatableSystems.Add(updatableSystem);
        }
    }

    public void Init(IWorld world)
    {
        foreach (var system in _initializableSystems)
        {
            system.Init(world);
        }
    }

    public void Update()
    {
        foreach (var system in _updatableSystems)
        {
            system.Update();
        }
    }
}