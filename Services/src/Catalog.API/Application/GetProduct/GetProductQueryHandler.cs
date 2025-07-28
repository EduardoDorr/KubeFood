using KubeFood.Catalog.API.Application.Models;
using KubeFood.Catalog.API.Domain;
using KubeFood.Core.Helpers;
using KubeFood.Core.Interfaces;
using KubeFood.Core.Results;

namespace KubeFood.Catalog.API.Application.GetProduct;

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