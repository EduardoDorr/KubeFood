namespace KubeFood.Core.Results.Api;

public sealed record ApiError(string Title, string Detail, int Status, string Type)
{
    public static readonly ApiError None = new(string.Empty, string.Empty, default, string.Empty);
}