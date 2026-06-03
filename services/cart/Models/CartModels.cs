namespace CartService.Models
{
    public class CartItem
    {
        public int Qty { get; set; }
        public string? Sku { get; set; }
        public string? Name { get; set; }
        public double Price { get; set; }
        public double Subtotal { get; set; }
    }

    public class Cart
    {
        public double Total { get; set; }
        public double Tax { get; set; }
        public List<CartItem> Items { get; set; } = new();
    }

    public class Product
    {
        public string? Sku { get; set; }
        public string? Name { get; set; }
        public double Price { get; set; }
        public int Instock { get; set; }
    }

    public class HealthStatus
    {
        public string? App { get; set; }
        public bool Redis { get; set; }
    }
}
