namespace NetCode.Ecs.Benchmarks;

public interface IEcsWrapper<T>
{
    ref T Add(int id);

    ref T Get(int id);

    void Delete(int id);
}