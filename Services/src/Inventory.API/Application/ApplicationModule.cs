using System.Reflection;

using KubeFood.Core;
using KubeFood.Inventory.API.Application;
using KubeFood.Inventory.API.Application.AddStockToItem;
using KubeFood.Inventory.API.Application.CreateItem;
using KubeFood.Inventory.API.Application.DeleteItem;
using KubeFood.Inventory.API.Application.GetItemById;
using KubeFood.Inventory.API.Application.GetItemByProductId;
using KubeFood.Inventory.API.Application.GetItems;
using KubeFood.Inventory.API.Application.RemoveStockFromItem;

namespace KubeFood.Inventory.API.Application;

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
        services.AddScoped<IGetItemsQueryHandler, GetItemsQueryHandler>();
        services.AddScoped<IGetItemByIdQueryHandler, GetItemByIdQueryHandler>();
        services.AddScoped<IGetItemByProductIdQueryHandler, GetItemByProductIdQueryHandler>();
        services.AddScoped<ICreateItemCommandHandler, CreateItemCommandHandler>();
        services.AddScoped<IAddStockToItemCommandHandler, AddStockToitemCommandHandler>();
        services.AddScoped<IRemoveStockFromItemCommandHandler, RemoveStockFromItemCommandHandler>();
        services.AddScoped<IDeleteItemCommandHandler, DeleteItemCommandHandler>();

        return services;
    }

    private static IServiceCollection AddEventHandlers(this IServiceCollection services)
    {
        services.AddEventHandlers(Assembly.GetExecutingAssembly());

        return services;
    }
}