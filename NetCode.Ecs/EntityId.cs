namespace NetCode.Ecs;

public readonly struct EntityId
{
    public readonly ushort Id;

    public EntityId(ushort id)
    {
        Id = id;
    }

    public override string ToString()
    {
        return Id.ToString();
    }
}