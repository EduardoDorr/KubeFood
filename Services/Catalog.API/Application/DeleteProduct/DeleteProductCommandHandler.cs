using Catalog.API.Domain;

using Core.Interfaces;
using Core.Persistence.UnitOfWork;
using Core.Results;

namespace Catalog.API.Application.DeleteProduct;

public interface IDeleteProductCommandHandler
    : IRequestHandler<DeleteProductCommand, Result>
{ }

public class DeleteProductCommandHandler : IDeleteProductCommandHandler
{
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteProductCommandHandler(IProductRepository productRepository, IUnitOfWork unitOfWork)
    {
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> HandleAsync(DeleteProductCommand request, CancellationToken cancellationToken = default)
    {
        var product =
            await _productRepository
                .GetByIdAsync(
                    request.Uiid,
                    cancellationToken);

        if (product is null)
            return Result.Fail(ProductError.NotFound);

        product.Deactivate();

        _productRepository.Update(product);
        var saveResult = await _unitOfWork.SaveChangesAsync(cancellationToken);

        if (saveResult <= 0)
            return Result.Fail(ProductError.CannotDelete);

        return Result.Ok();
    }
}