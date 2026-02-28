using Core.Entities;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController(ILogger<ProductsController> logger, StoreContext context) : ControllerBase
    {
        private readonly ILogger<ProductsController> _logger = logger;
        private readonly StoreContext _context = context;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts(CancellationToken cancellationToken = default)
        {
            return await _context.Products.ToListAsync(cancellationToken);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Product>> GetProduct(int id, CancellationToken cancellationToken = default)
        {
            var product = await _context.Products.FindAsync(id, cancellationToken);

            if (product is null) return NotFound();

            return product;
        }

        [HttpPost]
        public async Task<ActionResult<Product>> CreateProduct(Product product, CancellationToken cancellationToken = default)
        {
            _context.Products.Add(product);

            await _context.SaveChangesAsync(cancellationToken);

            return product;
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> UpdateProduct(int id, Product product, CancellationToken cancellationToken = default)
        {
            if (id != product.Id || !ProductExists(id)) return BadRequest("Cannot update this product");

            _context.Entry(product).State = EntityState.Modified;

            await _context.SaveChangesAsync(cancellationToken);

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteProduct(int id, CancellationToken cancellationToken = default)
        {
            var product = await _context.Products.FindAsync(id, cancellationToken);

            if (product is null) return NotFound();

            _context.Products.Remove(product);

            await _context.SaveChangesAsync(cancellationToken);

            return NoContent();
        }

        private bool ProductExists(int id) => _context.Products.Any(x => x.Id == id);
    }
}