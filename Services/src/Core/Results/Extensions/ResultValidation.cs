using KubeFood.Core.Results.Base;
using KubeFood.Core.Results.Errors;

namespace KubeFood.Core.Results.Extensions;

public static class ResultValidation
{
    /// <summary>
    /// Execute validators and returns first validation error (fail-fast)
    /// </summary>
    /// <returns>
    /// Returns first validation error, if any validator fail, otherwise, returns a <see cref="Result.Ok()"/>
    /// </returns>
    public static Result ValidateFailFast(params Func<Result>[] validators)
    {
        foreach (var validator in validators)
        {
            var result = validator();
            if (!result.Success)
                return result;
        }

        return Result.Ok();
    }

    /// <summary>
    /// Execute all validators and returns all validation errors, if any
    /// </summary>
    /// <returns>
    /// Returns a <see cref="Result.Fail(IEnumerable{IError})"/>, if any validator fail, otherwise, returns a <see cref="Result.Ok()"/>
    /// </returns>
    public static Result ValidateCollectErrors(params Func<Result>[] validators)
    {
        var errors = new List<IError>();

        foreach (var validator in validators)
        {
            var result = validator();
            if (!result.Success)
                errors.AddRange(result.Errors);
        }

        return errors.Count == 0
            ? Result.Ok()
            : Result.Fail(errors);
    }
}