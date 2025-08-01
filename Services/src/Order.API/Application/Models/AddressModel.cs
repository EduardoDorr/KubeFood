using KubeFood.Core.ValueObjects;

namespace KubeFood.Order.API.Application.Models;

public sealed record AddressModel(
    string Street,
    string City,
    string State,
    string Country,
    string ZipCode);

public static class AddressModelExtensions
{
    public static Address ToAddress(this AddressModel model)
        => Address.Create(
            model.Street,
            model.City,
            model.State,
            model.Country,
            model.ZipCode).Value!;
}