using KubeFood.Core.ValueObjects;

namespace KubeFood.Core.Entities;

public interface ILogin
{
    public Password Password { get; }
}