using KubeFood.Inventory.API.Api.Configuration;

await WebApplication.CreateBuilder(args)
    .ConfigureApplicationServices().Build()
    .ConfigureApplicationPipeline();
