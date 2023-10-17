using System.Runtime.Serialization;

namespace NetCode.Ecs;

public class ComponentNotFoundException : Exception
{
    public ComponentNotFoundException(EntityId entityId) : base(entityId.ToString())
    {
    }

    protected ComponentNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}