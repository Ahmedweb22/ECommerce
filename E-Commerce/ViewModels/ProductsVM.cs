namespace E_Commerce.ViewModels
{
    public class ProductsVM
    {
        public IEnumerable<Product> Products { get; set; } 
        public IEnumerable<Catgeory> Categories { get; set; }
        public IEnumerable<Brand> Brands { get; set; }
        public double TotalPages { get; set; }
        public int CurrentPage { get; set; }
        public ProductFilterVM FilterVM { get; set; }
    }
}
