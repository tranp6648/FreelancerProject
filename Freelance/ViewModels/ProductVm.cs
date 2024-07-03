namespace PhinaMart.ViewModels
{
    public class ProductVm
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public decimal Price { get; set; }
        public string DescriptionShort { get; set; }
        public string NameCategory { get; set; }
    }

    public class DetailProductVm
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public decimal Price { get; set; }
        public string DescriptionShort { get; set; }
        public string NameCategory { get; set; }
        public string Description { get; set; }
        public int StarRating { get; set; }
        public int StockQuantity { get; set; }
       

    }
}