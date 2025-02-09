using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductApiServices.Data;
using ProductApiServices.Models;
namespace ProductApiServices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly ProductContext _context;
        private readonly ILogger<ProductController> _logger;
        public ProductController(ProductContext context, ILogger<ProductController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/Products/ProductList
        [HttpGet]
        [Route("ProductList", Name = "ProductList")]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            List<Product> products = new List<Product>();
            try
            {
                products = await _context.Products.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting products.");
                return StatusCode(500, "Internal server error");
            }
            return products;
        }

        // GET: api/Products/GetProductById/5
        [HttpGet]
        [Route("GetProductById/{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            Product productDetails = new Product();
            try
            {
                productDetails = await _context.Products.FindAsync(id);

                if (productDetails == null)
                {
                    _logger.LogWarning("Product with id {ProductId} not found.", id);
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting the product with id {ProductId}.", id);
                return StatusCode(500, "Internal server error");
            }
            return productDetails;
        }

        // POST: api/Products/AddProduct
        [HttpPost]
        [Route("AddProduct")]
        public async Task<ActionResult<Product>> PostProduct(Product product)
        {
            try
            {
                _context.Products.Add(product);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Product with id {ProductId} created.", product.ProductId);
                return CreatedAtAction(nameof(GetProduct), new { id = product.ProductId }, product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating the product.");
                return StatusCode(500, "Internal server error");
            }
        }

        // PUT: api/Products/Update/5
        [HttpPut]
        [Route("Update/{id}")]
        public async Task<IActionResult> PutProduct(int id, Product product)
        {
            if (id != product.ProductId)
            {
                _logger.LogWarning("Product id mismatch: {ProductId} != {Product.Id}", id, product.ProductId);
                return BadRequest();
            }

            _context.Entry(product).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                _logger.LogInformation("Product with id {ProductId} updated.", id);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!ProductExists(id))
                {
                    _logger.LogWarning("Product with id {ProductId} not found.", id);
                    return NotFound();
                }
                else
                {
                    _logger.LogError(ex, "A concurrency error occurred while updating the product with id {ProductId}.", id);
                    return StatusCode(500, "Internal server error");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the product with id {ProductId}.", id);
                return StatusCode(500, "Internal server error");
            }
            return NoContent();
        }

        // DELETE: api/Products/Delete/5

        [HttpDelete]
        [Route("Delete/{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            try
            {
                var product = await _context.Products.FindAsync(id);
                if (product == null)
                {
                    _logger.LogWarning("Product with id {ProductId} not found.", id);
                    return NotFound();
                }

                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Product with id {ProductId} deleted.", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the product with id {ProductId}.", id);
                return StatusCode(500, "Internal server error");
            }
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ProductId == id);
        }

    }
}
