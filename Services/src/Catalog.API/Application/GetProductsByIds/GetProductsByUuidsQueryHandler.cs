using Catalog.API.Application.Models;
using Catalog.API.Domain;

using Core.Helpers;
using Core.Interfaces;
using Core.Models.Pagination;
using Core.Results;

using MongoDB.Bson;

namespace Catalog.API.Application.GetProductsByIds;

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
            .Select(HashIdHelper.DecodeObjectId)
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