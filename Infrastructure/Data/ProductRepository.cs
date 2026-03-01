using Core.Entities;
using Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class ProductRepository(StoreContext context) : IProductRepository
    {
        private readonly StoreContext _context = context;
        private const string PriceAsc = "priceAsc";
        private const string PriceDesc = "priceDesc";

        public void AddProduct(Product product) => _context.Products.Add(product);

        public void DeleteProduct(Product product) => _context.Products.Remove(product);

        public async Task<IReadOnlyList<string>> GetBrandsAsync(CancellationToken cancellationToken = default) => await _context.Products
            .Select(x => x.Brand)
            .Distinct()
            .ToListAsync(cancellationToken);
        public async Task<IReadOnlyList<string>> GetTypesAsync(CancellationToken cancellationToken = default) => await _context.Products
           .Select(x => x.Type)
           .Distinct()
           .ToListAsync(cancellationToken);

        public async Task<Product?> GetProductByIdAsync(int id, CancellationToken cancellationToken = default) => await _context.Products.FindAsync(id, cancellationToken);

        public async Task<IReadOnlyList<Product>> GetProductsAsync(string? brand, string? type, string? sort, CancellationToken cancellationToken = default)
        {
            var query = _context.Products.AsQueryable();

            if (!string.IsNullOrWhiteSpace(brand)) query = query.Where(x => x.Brand == brand);
            if (!string.IsNullOrWhiteSpace(type)) query = query.Where(x => x.Type == type);

            query = sort switch
            {
                PriceAsc => query.OrderBy(x => x.Price),
                PriceDesc => query.OrderByDescending(x => x.Price),
                _ => query.OrderBy(x => x.Name)
            };

            return await query.ToListAsync(cancellationToken);
        }

        public bool ProductExists(int id) => _context.Products.Any(x => x.Id == id);

        public async Task<bool> SaveChangesAsync(CancellationToken cancellationToken = default) => await _context.SaveChangesAsync(cancellationToken) > 0;

        public void UpdateProduct(Product product) => _context.Entry(product).State = EntityState.Modified;
    }
}