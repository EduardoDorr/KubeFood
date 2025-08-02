using System.Reflection;

using KubeFood.Core;
using KubeFood.Order.API.Application.CancelOrder;
using KubeFood.Order.API.Application.CreateOrder;
using KubeFood.Order.API.Application.GetOrder;
using KubeFood.Order.API.Application.GetOrders;
using KubeFood.Order.API.Application.GetOrdersByCustomer;
using KubeFood.Order.API.Application.UpdateOrder;

namespace KubeFood.Order.API.Application;

public static class ApplicationModule
{
    public static IServiceCollection AddApplicationModule(this IServiceCollection services)
    {
        services
            .AddServices()
            .AddDomainEventHandlers();

        return services;
    }

    private static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IGetOrderQueryHandler, GetOrderQueryHandler>();
        services.AddScoped<IGetOrdersQueryHandler, GetOrdersQueryHandler>();
        services.AddScoped<IGetOrdersByCustomerQueryHandler, GetOrdersByCustomerQueryHandler>();
        services.AddScoped<ICreateOrderCommandHandler, CreateOrderCommandHandler>();
        services.AddScoped<IUpdateOrderCommandHandler, UpdateOrderCommandHandler>();
        services.AddScoped<ICancelOrderCommandHandler, CancelOrderCommandHandler>();

        return services;
    }

    private static IServiceCollection AddDomainEventHandlers(this IServiceCollection services)
    {
        services.AddDomainEventHandlers(Assembly.GetExecutingAssembly());

        return services;
    }
}