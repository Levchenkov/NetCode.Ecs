namespace NetCode.Ecs;

public interface IWorld
{
    SparseSet<T> GetSparseSet<T>() where T : struct;
}