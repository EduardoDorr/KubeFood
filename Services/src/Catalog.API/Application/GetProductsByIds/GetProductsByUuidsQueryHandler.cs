using KubeFood.Catalog.API.Application.Models;
using KubeFood.Catalog.API.Domain;
using KubeFood.Core.Helpers;
using KubeFood.Core.Interfaces;
using KubeFood.Core.Results.Base;

using MongoDB.Bson;

namespace KubeFood.Catalog.API.Application.GetProductsByIds;

public interface IGetProductsByUuidsQueryHandler
    : IRequestHandler<GetProductsByUuidsQuery, Result<IEnumerable<ProductViewModel>>>
{ }

public class GetProductsByUuidsQueryHandler : IGetProductsByUuidsQueryHandler
{
    private readonly IProductRepository _productRepository;

    public GetProductsByUuidsQueryHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<Result<IEnumerable<ProductViewModel>>> HandleAsync(GetProductsByUuidsQuery request, CancellationToken cancellationToken = default)
    {
        var ids = request.ProductUuids
            .Select(HashIdHelper.DecodeHashId<ObjectId>)
            .Where(id => id != ObjectId.Empty)
            .ToList();

        var products =
            await _productRepository
                .GetByIdsAsync(
                    ids,
                    cancellationToken);

        return Result.Ok(products.ToModel());
    }
}