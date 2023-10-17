using System.Runtime.Serialization;

namespace NetCode.Ecs;

public class MaxCapacityException : Exception
{
    public MaxCapacityException(Type type) : base(type.ToString())
    {
    }

    protected MaxCapacityException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}