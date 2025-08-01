using KubeFood.Core.Models.Pagination;
using KubeFood.Core.Results;
using KubeFood.Order.API.Application.CancelOrder;
using KubeFood.Order.API.Application.CreateOrder;
using KubeFood.Order.API.Application.GetOrder;
using KubeFood.Order.API.Application.GetOrders;
using KubeFood.Order.API.Application.GetOrdersByCustomer;
using KubeFood.Order.API.Application.Models;
using KubeFood.Order.API.Application.UpdateOrder;

namespace KubeFood.Order.API.Api.Endpoints;

public static class OrderEndpoint
{
    private const string ROUTE = "/api/v1/order";

    public static WebApplication MapOrderEndpoints(this WebApplication app)
    {
        var group = app.MapGroup(ROUTE)
            .WithTags("Orders");

        group.MapGet("/", async (
            [AsParameters] PaginationParameters pagination,
            IGetOrdersQueryHandler service,
            CancellationToken cancellationToken) =>
        {
            var result = await service.HandleAsync(new(pagination), cancellationToken);

            return result.Match(
                onSuccess: success => Results.Ok(result),
                onFailure: error => error.ToProblemDetails());
        })
        .Produces<PaginationResult<PaginationResult<Domain.Order>>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status500InternalServerError);

        group.MapGet("/customer/{customerId}", async (
            int customerId,
            [AsParameters] PaginationParameters pagination,
            IGetOrdersByCustomerQueryHandler service,
            CancellationToken cancellationToken) =>
        {
            var result = await service.HandleAsync(new(customerId, pagination), cancellationToken);

            return result.Match(
                onSuccess: success => Results.Ok(result.Value),
                onFailure: error => error.ToProblemDetails());
        })
        .Produces<PaginationResult<OrderViewModel>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status500InternalServerError);

        group.MapGet("/{id}", async (
            int id,
            IGetOrderQueryHandler service,
            CancellationToken cancellationToken) =>
        {
            var result = await service.HandleAsync(new(id), cancellationToken);

            return result.Match(
                onSuccess: success => Results.Ok(result.Value),
                onFailure: error => error.ToProblemDetails());
        })
        .Produces<OrderViewModel>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status500InternalServerError);

        group.MapPost("/", async (
            CreateOrderCommand command,
            ICreateOrderCommandHandler service,
            CancellationToken cancellationToken) =>
        {
            if (command is null)
                return Results.BadRequest("Order cannot be null.");

            var result = await service.HandleAsync(command, cancellationToken);

            return result.Match(
                onSuccess: success => Results.Created($"{ROUTE}/{success}", command),
                onFailure: error => error.ToProblemDetails());
        })
        .Produces<int>(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status500InternalServerError);

        group.MapPut("/{id}", async (
            int id,
            UpdateOrderInputModel model,
            IUpdateOrderCommandHandler service,
            CancellationToken cancellationToken) =>
        {
            if (model is null)
                return Results.BadRequest("Order cannot be null.");

            var result = await service.HandleAsync(model.ToCommand(id), cancellationToken);

            return result.Match(
                onSuccess: () => Results.NoContent(),
                onFailure: error => error.ToProblemDetails());
        })
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status500InternalServerError);

        group.MapDelete("/{id}", async (
            int id,
            ICancelOrderCommandHandler service,
            CancellationToken cancellationToken) =>
        {
            var result = await service.HandleAsync(new(id), cancellationToken);

            return result.Match(
                onSuccess: () => Results.NoContent(),
                onFailure: error => error.ToProblemDetails());
        })
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status500InternalServerError);

        return app;
    }
}