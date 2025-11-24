using KubeFood.Core.ValueObjects;

namespace KubeFood.Core.Entities.Interfaces;

public interface ILogin
{
    public Password Password { get; }
}