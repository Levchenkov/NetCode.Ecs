namespace NetCode.Ecs;

public interface IInitializableSystem
{
    void Init(IWorld world);
}