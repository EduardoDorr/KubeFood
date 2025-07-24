using Catalog.API.Application.Models;
using Catalog.API.Domain;

using Core.Helpers;
using Core.Interfaces;
using Core.Results;

namespace Catalog.API.Application.GetProduct;

public interface IGetProductQueryHandler
    : IRequestHandler<GetProductQuery, Result<ProductViewModel?>>
{ }

public class GetProductQueryHandler : IGetProductQueryHandler
{
    private readonly IProductRepository _productRepository;

    public GetProductQueryHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<Result<ProductViewModel?>> HandleAsync(GetProductQuery request, CancellationToken cancellationToken = default)
    {
        var product =
            await _productRepository
                .GetByIdAsync(
                    request.Uiid.DecodeObjectId(),
                    cancellationToken);

        if (product is null)
            return Result.Fail<ProductViewModel?>(ProductError.NotFound);

        return Result.Ok(product.ToModel());
    }
}