namespace E_Commerce.ViewModels
{
    public class CategoriesVM
    {
        public IEnumerable<Catgeory> Categories { get; set; }
        public double TotalPages { get; set; }
        public int CurrentPage { get; set; }
    }
}
