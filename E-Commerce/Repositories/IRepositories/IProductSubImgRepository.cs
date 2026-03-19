namespace E_Commerce.Repositories.IRepositories
{
    public interface IProductSubImgRepository : IRepository<ProductSubImg>
    {
        void DeleteRange(List<ProductSubImg> productSubImgs);
    }
}
