using Catalog.API.Application.Models;
using Catalog.API.Domain;

using Core.Interfaces;
using Core.Models.Pagination;
using Core.Results;

namespace Catalog.API.Application.GetProducts;

public interface IGetProductsQueryHandler
    : IRequestHandler<GetProductsQuery, Result<PaginationResult<ProductViewModel>>>
{ }

public class GetProductsQueryHandler : IGetProductsQueryHandler
{
    private readonly IProductRepository _productRepository;

    public GetProductsQueryHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<Result<PaginationResult<ProductViewModel>>> HandleAsync(GetProductsQuery request, CancellationToken cancellationToken = default)
    {
        var products = 
            await _productRepository
                .GetAllAsync(
                    request.Pagination.Page,
                    request.Pagination.PageSize,
                    cancellationToken);

        var paginatedProducts =
            products
                .Map(products.Data.ToModel().ToList());

        return Result.Ok(paginatedProducts);
    }
}