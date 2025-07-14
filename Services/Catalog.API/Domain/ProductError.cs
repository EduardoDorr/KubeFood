using Core.Results.Errors;

namespace Catalog.API.Domain;

public static class ProductError
{
    public static Error NotFound => 
        new("ProductError.NotFound", "Could not find a product with this uuid", ErrorType.NotFound);

    public static Error CannotCreate =>
        new("ProductError.CannotCreate", "Something prevented from creating a new product", ErrorType.Failure);

    public static Error CannotUpdate =>
        new("ProductError.CannotUpdate", "Something prevented from updating a product", ErrorType.Failure);

    public static Error CannotDelete =>
        new("ProductError.CannotDelete", "Something prevented from deleting a product", ErrorType.Failure);
}