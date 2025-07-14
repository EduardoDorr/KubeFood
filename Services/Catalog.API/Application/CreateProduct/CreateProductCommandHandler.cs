
using Catalog.API.Application.Models;
using Catalog.API.Domain;

using Core.Interfaces;
using Core.Persistence.UnitOfWork;
using Core.Results;

using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Catalog.API.Application.CreateProduct;

public interface ICreateProductCommandHandler
    : IRequestHandler<CreateProductCommand, Result<string>>
{ }

public class CreateProductCommandHandler : ICreateProductCommandHandler
{
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateProductCommandHandler(IProductRepository productRepository, IUnitOfWork unitOfWork)
    {
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<string>> HandleAsync(CreateProductCommand request, CancellationToken cancellationToken = default)
    {
        var product = request.ToProduct();

        _productRepository.Create(product);
        var createResult = await _unitOfWork.SaveChangesAsync(cancellationToken);

        if (createResult > 0)
            return Result.Fail<string>(ProductError.CannotCreate);

        product.SetUuid(product.Id);

        _productRepository.Update(product);
        var updateResult = await _unitOfWork.SaveChangesAsync(cancellationToken);

        if (updateResult > 0)
            return Result.Fail<string>(ProductError.CannotUpdate);

        return Result.Ok(product.Uuid);
    }
}