using System.Net;

using KubeFood.Core.Results.Base;
using KubeFood.Core.Results.Extensions;

namespace KubeFood.Core.Results.Api;

public interface IApiResult
{
    public bool Success { get; }
    public int Status { get; }
    public IReadOnlyList<ApiError> Errors { get; }
    public IReadOnlyList<ApiInfo> Infos { get; }

    public IApiResult AddError(ApiError error);
    public IApiResult AddInfo(ApiInfo info);
}

public class ApiResult : IApiResult
{
    public bool Success => _errors.Count == 0;
    public int Status { get; private set; }
    public IReadOnlyList<ApiError> Errors => _errors.AsReadOnly();
    public IReadOnlyList<ApiInfo> Infos => _infos.AsReadOnly();

    protected readonly List<ApiError> _errors = [];
    protected readonly List<ApiInfo> _infos = [];

    public ApiResult(IResultBase result, HttpStatusCode statusCode)
    {
        Status = (int)statusCode;

        if (result.Errors is not null)
        {
            _errors.AddRange(result.Errors.ToApiError());
            Status = (_errors.Count > 0 ? _errors.MaxBy(e => e.Status)?.Status : Status) ?? Status;
        }
    }

    public ApiResult(HttpStatusCode statusCode, IEnumerable<ApiError> errors)
    {
        Status = (int)statusCode;

        if (errors is not null)
        {
            _errors.AddRange(errors);
            Status = (_errors.Count > 0 ? _errors.MaxBy(e => e.Status)?.Status : Status) ?? Status;
        }
    }

    public IApiResult SetStatus(int statusCode)
    {
        Status = statusCode;
        return this;
    }

    public IApiResult SetStatus(HttpStatusCode statusCode)
    {
        return SetStatus((int)statusCode);
    }

    public IApiResult AddError(ApiError error)
    {
        _errors.Add(error);
        return this;
    }

    public IApiResult AddInfo(ApiInfo info)
    {
        _infos.Add(info);
        return this;
    }

    public static ApiResult Ok() => new(HttpStatusCode.OK, []);
    public static ApiResult<TValue> Ok<TValue>(TValue value) => new(value, HttpStatusCode.OK, []);
}

public class ApiResult<TValue> : ApiResult
{
    public TValue? Value => _value;
    protected readonly TValue? _value;

    public ApiResult(IResultBase<TValue> result, HttpStatusCode statusCode)
        : base(result, statusCode)
    {
        _value = result.Value;
    }

    public ApiResult(TValue value, HttpStatusCode statusCode, IEnumerable<ApiError> errors)
        : base(statusCode, errors)
    {
        _value = value;
    }

    public static ApiResult<TValue> Ok<TValue>(TValue value) => new(value, HttpStatusCode.OK, []);
}