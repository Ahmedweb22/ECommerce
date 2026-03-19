namespace E_Commerce.ViewModels
{
    public class ProductUpdateResponseVM
    {
        public Product Product { get; set; } = null!;
        public IEnumerable<ProductSubImg> SubImg { get; set; }= null!;
        public IEnumerable<Catgeory> Categories { get; set; }
        public IEnumerable<Brand> Brands { get; set; }
    }
}
