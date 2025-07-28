using KubeFood.Catalog.API.Application.CreateProduct;
using KubeFood.Catalog.API.Application.DeleteProduct;
using KubeFood.Catalog.API.Application.GetProduct;
using KubeFood.Catalog.API.Application.GetProducts;
using KubeFood.Catalog.API.Application.GetProductsByIds;
using KubeFood.Catalog.API.Application.UpdateProduct;
using KubeFood.Catalog.API.Application.UpdateProductImage;

namespace KubeFood.Catalog.API.Application;

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