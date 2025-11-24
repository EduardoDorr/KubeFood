using KubeFood.Core.Results.Errors;

namespace KubeFood.Order.API.Domain;

public static class OrderError
{
    public static Error NotFound =>
        new("OrderError.NotFound", "Could not find a order with this id", ErrorType.NotFound);

    public static Error CannotCreate =>
        new("OrderError.CannotCreate", "Something prevented from creating a new order", ErrorType.Failure);

    public static Error CannotUpdate =>
        new("OrderError.CannotUpdate", "Something prevented from updating a order", ErrorType.Failure);

    public static Error CannotDelete =>
        new("OrderError.CannotDelete", "Something prevented from deleting a order", ErrorType.Failure);
}