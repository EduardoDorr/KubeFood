using KubeFood.Catalog.API.Application.Models;
using KubeFood.Catalog.API.Domain;
using KubeFood.Core.Helpers;
using KubeFood.Core.Interfaces;
using KubeFood.Core.Results.Base;

using MongoDB.Bson;

namespace KubeFood.Catalog.API.Application.GetByIdProduct;

public interface IGetProductQueryHandler
    : IRequestHandler<GetProductByUiidQuery, Result<ProductViewModel?>>
{ }

public class GetProductByIdQueryHandler : IGetProductQueryHandler
{
    private readonly IProductRepository _productRepository;

    public GetProductByIdQueryHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<Result<ProductViewModel?>> HandleAsync(GetProductByUiidQuery request, CancellationToken cancellationToken = default)
    {
        var product =
            await _productRepository
                .GetByIdAsync(
                    request.Uiid.DecodeHashId<ObjectId>(),
                    cancellationToken);

        if (product is null)
            return Result.Fail<ProductViewModel?>(ProductError.NotFound);

        return Result.Ok(product.ToModel());
    }
}