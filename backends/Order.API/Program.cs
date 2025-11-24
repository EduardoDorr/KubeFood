using KubeFood.Order.API.Api.Configuration;

await WebApplication.CreateBuilder(args)
    .ConfigureApplicationServices().Build()
    .ConfigureApplicationPipeline();