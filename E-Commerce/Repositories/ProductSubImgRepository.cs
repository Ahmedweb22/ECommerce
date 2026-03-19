namespace E_Commerce.Repositories
{
    public class ProductSubImgRepository : Repository<ProductSubImg> , IProductSubImgRepository
    {
        private ApplicationDbContext _context = new();
        public void DeleteRange(List<ProductSubImg> productSubImgs) 
        {
        _context.ProductSubImgs.RemoveRange(productSubImgs);
        }
    }
}
