namespace KubeFood.WebApp.Models.Produtcs;

public class Product
{
    public string id { get; set; } = null!;
    public string name { get; set; } = null!;
    public string description { get; set; } = null!;
    public int category { get; set; }
    public string imageUrl { get; set; } = null!;
    public int value { get; set; }
    public int weight { get; set; }
}