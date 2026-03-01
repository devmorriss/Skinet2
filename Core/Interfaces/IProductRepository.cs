using Core.Entities;

namespace Core.Interfaces
{
    public interface IProductRepository
    {
        Task<IReadOnlyList<Product>> GetProductsAsync(string? brand, string? type, string? sort, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<string>> GetBrandsAsync(CancellationToken cancellationToken = default);
        Task<IReadOnlyList<string>> GetTypesAsync(CancellationToken cancellationToken = default);
        Task<Product?> GetProductByIdAsync(int id, CancellationToken cancellationToken = default);
        void AddProduct(Product product);
        void UpdateProduct(Product product);
        void DeleteProduct(Product product);
        bool ProductExists(int id);
        Task<bool> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}