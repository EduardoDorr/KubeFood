﻿using Catalog.API.Application.CreateProduct;
using Catalog.API.Application.DeleteProduct;
using Catalog.API.Application.GetProduct;
using Catalog.API.Application.GetProducts;
using Catalog.API.Application.GetProductsByIds;
using Catalog.API.Application.UpdateProduct;
using Catalog.API.Application.UpdateProductImage;

namespace Catalog.API.Application;

public static class ApplicationModule
{
    public static IServiceCollection AddApplicationModule(this IServiceCollection services)
    {
        services
            .AddServices();

        return services;
    }

    private static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IGetProductQueryHandler, GetProductQueryHandler>();
        services.AddScoped<IGetProductsQueryHandler, GetProductsQueryHandler>();
        services.AddScoped<IGetProductsByUuidsQueryHandler, GetProductsByUuidsQueryHandler>();
        services.AddScoped<ICreateProductCommandHandler, CreateProductCommandHandler>();
        services.AddScoped<IUpdateProductCommandHandler, UpdateProductCommandHandler>();
        services.AddScoped<IDeleteProductCommandHandler, DeleteProductCommandHandler>();
        services.AddScoped<IUpdateProductImageCommandHandler, UpdateProductImageCommandHandler>();

        return services;
    }
}