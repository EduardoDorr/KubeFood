using Core.ValueObjects;

namespace Core.Entities;

public interface ILogin
{
    public Password Password { get; }
}