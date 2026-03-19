namespace E_Commerce.ViewModels
{
    public class ProductWithRelatedVM
    {
        public Product Product { get; set; }
        public List<Product> SameCategories { get; set; }
        public List<Product> SamePrices { get; set; }
        public List<Product> RelatedProducts { get; set; }
      

    }
}
