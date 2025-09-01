using KubeFood.Core.Results.Errors;

namespace KubeFood.Core.Results.Base;

public abstract class ValidationResultBase<TContext> : ResultBase
{
    public TContext Context => _context;

    protected TContext _context;

    protected ValidationResultBase(TContext context)
    {
        _context = context;
    }
}

public class ValidationResult<TContext> : ValidationResultBase<TContext>
{
    private ValidationResult(TContext context)
        : base(context) { }

    private ValidationResult(TContext context, IError error)
        : base(context)
    {
        AddError(error);
    }

    private ValidationResult(TContext context, IEnumerable<IError> errors)
        : base(context)
    {
        AddErrors(errors);
    }

    public static ValidationResult<TContext> Ok(TContext context)
        => new(context);

    public static ValidationResult<TContext> Fail(TContext context, IError error)
        => new(context, error);

    public static ValidationResult<TContext> Fail(TContext context, IEnumerable<IError> errors)
        => new(context, errors);
}

public static class ValidationResult
{
    public static ValidationResult<TContext> Ok<TContext>(TContext context)
        => ValidationResult<TContext>.Ok(context);

    public static ValidationResult<TContext> Fail<TContext>(TContext context, IError error)
        => ValidationResult<TContext>.Fail(context, error);

    public static ValidationResult<TContext> Fail<TContext>(TContext context, IEnumerable<IError> errors)
        => ValidationResult<TContext>.Fail(context, errors);
}