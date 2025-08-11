using System.Reflection;

using KubeFood.Catalog.API.Application.CreateProduct;
using KubeFood.Catalog.API.Application.DeleteProduct;
using KubeFood.Catalog.API.Application.GetByIdProduct;
using KubeFood.Catalog.API.Application.GetProducts;
using KubeFood.Catalog.API.Application.GetProductsByIds;
using KubeFood.Catalog.API.Application.UpdateProduct;
using KubeFood.Catalog.API.Application.UpdateProductImage;
using KubeFood.Core;

namespace KubeFood.Catalog.API.Application;

public static class ApplicationModule
{
    public static IServiceCollection AddApplicationModule(this IServiceCollection services)
    {
        services
            .AddServices()
            .AddEventHandlers();

        return services;
    }

    private static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IGetProductQueryHandler, GetProductByIdQueryHandler>();
        services.AddScoped<IGetProductsQueryHandler, GetProductsQueryHandler>();
        services.AddScoped<IGetProductsByUuidsQueryHandler, GetProductsByUuidsQueryHandler>();
        services.AddScoped<ICreateProductCommandHandler, CreateProductCommandHandler>();
        services.AddScoped<IUpdateProductCommandHandler, UpdateProductCommandHandler>();
        services.AddScoped<IDeleteProductCommandHandler, DeleteProductCommandHandler>();
        services.AddScoped<IUpdateProductImageCommandHandler, UpdateProductImageCommandHandler>();

        return services;
    }

    private static IServiceCollection AddEventHandlers(this IServiceCollection services)
    {
        services.AddEventHandlers(Assembly.GetExecutingAssembly());

        return services;
    }
}