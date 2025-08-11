namespace KubeFood.Core.Results.Api;

public sealed record ApiInfo(string Code, string Message)
{
    public static readonly ApiInfo None = new(string.Empty, string.Empty);
}