using KubeFood.Core.Results.Errors;

namespace KubeFood.Core.Results.Base;

public interface IResultBase
{
    IReadOnlyList<IError> Errors { get; }
    bool Success { get; }
}

public interface IResultBase<out TValue> : IResultBase
{
    public TValue? Value { get; }
}

public abstract class ResultBase : IResultBase
{
    public IReadOnlyList<IError> Errors => _errors.AsReadOnly();
    public bool Success => _errors.Count == 0;

    protected readonly List<IError> _errors = [];
}

public abstract class ResultBase<TValue> : ResultBase, IResultBase<TValue>
{
    public TValue? Value => _value;

    protected TValue? _value;
}