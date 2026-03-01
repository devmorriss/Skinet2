using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController(ILogger<ProductsController> logger, IProductRepository productRepository) : ControllerBase
    {
        private readonly ILogger<ProductsController> _logger = logger;
        private readonly IProductRepository _productRepository = productRepository;

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<Product>>> GetProducts(
            string? brand,
            string? type,
            string? sort,
            CancellationToken cancellationToken = default
        )
        {
            return Ok(await _productRepository.GetProductsAsync(brand, type, sort, cancellationToken));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Product>> GetProduct(int id, CancellationToken cancellationToken = default)
        {
            var product = await _productRepository.GetProductByIdAsync(id, cancellationToken);

            if (product is null) return NotFound();

            return product;
        }

        [HttpPost]
        public async Task<ActionResult<Product>> CreateProduct(Product product, CancellationToken cancellationToken = default)
        {
            _productRepository.AddProduct(product);

            if (await _productRepository.SaveChangesAsync(cancellationToken)) return CreatedAtAction("GetProduct", new { id = product.Id }, product);

            return BadRequest("Problem creating product");
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> UpdateProduct(int id, Product product, CancellationToken cancellationToken = default)
        {
            if (id != product.Id || !ProductExists(id)) return BadRequest("Cannot update this product");

            _productRepository.UpdateProduct(product);

            if (await _productRepository.SaveChangesAsync(cancellationToken)) return NoContent();

            return BadRequest("Problem updating product");
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteProduct(int id, CancellationToken cancellationToken = default)
        {
            var product = await _productRepository.GetProductByIdAsync(id, cancellationToken);

            if (product is null) return NotFound();

            _productRepository.DeleteProduct(product);

            if (await _productRepository.SaveChangesAsync(cancellationToken)) return NoContent();

            return BadRequest("Problem deleting product");
        }

        [HttpGet("brands")]
        public async Task<ActionResult<IReadOnlyList<string>>> GetBrands(CancellationToken cancellationToken = default)
        {
            return Ok(await _productRepository.GetBrandsAsync(cancellationToken));
        }

        [HttpGet("types")]
        public async Task<ActionResult<IReadOnlyList<string>>> GetTypes(CancellationToken cancellationToken = default)
        {
            return Ok(await _productRepository.GetTypesAsync(cancellationToken));
        }


        private bool ProductExists(int id) => _productRepository.ProductExists(id);
    }
}