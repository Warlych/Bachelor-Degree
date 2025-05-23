namespace Messages.Broker.Common.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class QueueAttribute : Attribute
{
    private readonly string _queue;
    private readonly bool _durable;
    private readonly bool _exclusive;
    private readonly bool _autoDeleteOnIdle;

    public QueueAttribute(string queue, bool durable = true, bool exclusive = default, bool autoDeleteOnIdle = default)
    {
        _queue = queue;
        _durable = durable;
        _exclusive = exclusive;
        _autoDeleteOnIdle = autoDeleteOnIdle;
    }

    public string Queue => _queue;
    public bool Durable => _durable;
    public bool Exclusive => _exclusive;
    public bool AutoDeleteOnIdle => _autoDeleteOnIdle;
}
