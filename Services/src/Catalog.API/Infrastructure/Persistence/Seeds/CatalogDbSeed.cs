using Catalog.API.Domain;

using Microsoft.EntityFrameworkCore;

namespace Catalog.API.Infrastructure.Persistence.Seeds;

public static class CatalogDbSeed
{
    public static async Task<WebApplication> SeedAsync(this WebApplication app)
    {
        var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();

        if (await dbContext.Products.AnyAsync())
            return app;

        await SeedProductsAsync(dbContext);

        return app;
    }

    private static async Task SeedProductsAsync(CatalogDbContext context)
    {
        var products = new List<Product>
{
            new(
                name: "Cheeseburger Clássico",
                description: "Pão brioche, hambúrguer 150g, queijo cheddar, alface, tomate, maionese especial.",
                category: ProductCategory.Lanche,
                value: 24.90m,
                weight: 250.0m
            ),
            new(
                name: "X-Bacon Supremo",
                description: "Pão australiano, hambúrguer 200g, queijo prato, bacon crocante, cebola caramelizada e barbecue.",
                category: ProductCategory.Lanche,
                value: 29.90m,
                weight: 320.0m
            ),
            new(
                name: "Batata Frita com Cheddar e Bacon",
                description: "Batata frita crocante coberta com queijo cheddar derretido e pedaços de bacon.",
                category: ProductCategory.Acompanhamento,
                value: 18.50m,
                weight: 180.0m
            ),
            new(
                name: "Nuggets de Frango (8 unidades)",
                description: "Nuggets crocantes de frango com molho barbecue ou honey mustard.",
                category: ProductCategory.Acompanhamento,
                value: 14.00m,
                weight: 150.0m
            ),
            new(
                name: "Milkshake de Chocolate",
                description: "Milkshake cremoso de chocolate com chantilly e calda extra.",
                category: ProductCategory.Bebida,
                value: 17.00m,
                weight: 400.0m
            ),
            new(
                name: "Refrigerante Lata (350ml)",
                description: "Escolha entre Coca-Cola, Guaraná ou Sprite.",
                category: ProductCategory.Bebida,
                value: 6.00m,
                weight: 350.0m
            )
        };

        foreach (var product in products)
        {
            context.Products.Add(product);
            await context.SaveChangesAsync();

            product.SetUuid(product.Id);
            context.Products.Update(product);
            await context.SaveChangesAsync();
        }
    }
}