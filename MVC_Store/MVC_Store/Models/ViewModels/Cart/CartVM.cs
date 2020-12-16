namespace MVC_Store.Models.ViewModels.Cart
{
    public class CartVM
    {
        public int ProductId { get; set; }

        public string Name { get; set; }

        public int Quantity { get; set; }

        public decimal Price { get; set; }

        public decimal Total => Quantity * Price;

        public string Image { get; set; }
    }
}