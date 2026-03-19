namespace E_Commerce.ViewModels
{
    public class BrandsVM
    {
        public IEnumerable<Brand> Brands { get; set; }
        public int CurrentPage { get; set; }
        public double TotalPages { get; set; }
    }
}
