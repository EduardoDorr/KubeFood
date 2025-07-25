﻿using Catalog.API.Domain;

using Core.Helpers;
using Core.Interfaces;
using Core.Persistence.UnitOfWork;
using Core.Results;

namespace Catalog.API.Application.UpdateProduct;

public interface IUpdateProductCommandHandler
    : IRequestHandler<UpdateProductCommand, Result>
{ }

public class UpdateProductCommandHandler : IUpdateProductCommandHandler
{
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateProductCommandHandler(IProductRepository productRepository, IUnitOfWork unitOfWork)
    {
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> HandleAsync(UpdateProductCommand request, CancellationToken cancellationToken = default)
    {
        var product =
            await _productRepository
                .GetByIdAsync(
                    request.Uiid.DecodeObjectId(),
                    cancellationToken);

        if (product is null)
            return Result.Fail(ProductError.NotFound);

        var result =
            product.Update(
                request.Name,
                request.Description,
                request.Category,
                request.Value,
                request.Weight);

        if (!result.Success)
            return Result.Fail(result.Errors);

        _productRepository.Update(product);
        var updateResult = await _unitOfWork.SaveChangesAsync(cancellationToken);

        if (updateResult <= 0)
            return Result.Fail(ProductError.CannotUpdate);

        return Result.Ok();
    }
}