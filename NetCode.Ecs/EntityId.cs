namespace NetCode.Ecs;

public readonly struct EntityId
{
    public const int MaxEntitiesCount = ushort.MaxValue;

    public readonly ushort Id;

    public EntityId(ushort id)
    {
        Id = id;
    }

    public override string ToString()
    {
        return Id.ToString();
    }

    public static implicit operator EntityId(ushort value)
    {
        return new EntityId(value);
    }
}