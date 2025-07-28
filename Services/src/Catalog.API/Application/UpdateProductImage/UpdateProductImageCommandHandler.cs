using KubeFood.Catalog.API.Domain;
using KubeFood.Core.Helpers;
using KubeFood.Core.Interfaces;
using KubeFood.Core.Persistence.UnitOfWork;
using KubeFood.Core.Results;

namespace KubeFood.Catalog.API.Application.UpdateProductImage;

public interface IUpdateProductImageCommandHandler
    : IRequestHandler<UpdateProductImageCommand, Result>
{ }

public class UpdateProductImageCommandHandler : IUpdateProductImageCommandHandler
{
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateProductImageCommandHandler(
        IProductRepository productRepository,
        IUnitOfWork unitOfWork)
    {
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> HandleAsync(UpdateProductImageCommand request, CancellationToken cancellationToken = default)
    {
        var product =
            await _productRepository
                .GetByIdAsync(
                    request.Uiid.DecodeObjectId(),
                    cancellationToken);

        if (product is null)
            return Result.Fail(ProductError.NotFound);

        var result =
            product.SetImage(request.ImageUrl);

        if (!result.Success)
            return Result.Fail(result.Errors);

        _productRepository.Update(product);
        var updateResult = await _unitOfWork.SaveChangesAsync(cancellationToken);

        if (updateResult <= 0)
            return Result.Fail(ProductError.CannotUpdate);

        return Result.Ok();
    }
}