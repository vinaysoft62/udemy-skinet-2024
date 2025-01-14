using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController(IProductRepository repo) : ControllerBase
    {
        [HttpGet("GetAllProducts")] 
        public async Task<ActionResult<IReadOnlyList<Product>>> GetProducts(string? brand, string? type,
        string? sort) 
        //method name can be anything but only how we are calling/mapping is the point.
        {
            return Ok(await repo.GetProductsAsync(brand, type, sort));
        }

        [HttpGet("ThisProduct/{id:int}")] // api/products/2
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var product = await repo.GetProductByIdAsync(id);
            if(product == null){return NotFound();}

            return product;
        }

        [HttpPost]
        public async Task<ActionResult<Product>> CreateProduct(Product product)
        {
            repo.AddProduct(product);

           if(await repo.SaveChangesAsync())
           {
            return CreatedAtAction("GetProduct", new {id = product.Id}, product);
           }

            return BadRequest("Problem creating product");
        }


        [HttpPut("{id:int}")]
        public async Task<ActionResult> UpdateProduct(int id, Product product)
        {
            if(product.Id != id || !ProductExists(id) )
            {
                return BadRequest("Cannont update this product.");
            }

            repo.UpdateProduct(product);
            if(await repo.SaveChangesAsync())
            {
                return NoContent();
            }

            return BadRequest("Problem updating the product");
        }

        private bool ProductExists(int id)
        {
            return repo.ProductExists(id);
        }

        [HttpGet("brands")]
        public async Task<ActionResult<IReadOnlyList<string>>> GetBrands()
        {
            return Ok(await repo.GetBrandsAsync());
        }

        [HttpGet("types")]
        public async Task<ActionResult<IReadOnlyList<string>>> GetTypes()
        {
            return Ok(await repo.GetTypesAsync());
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteProduct(int id)
        {
            var product = await repo.GetProductByIdAsync(id);
            if(product == null) return NotFound();

            repo.DeleteProduct(product);
           if(await repo.SaveChangesAsync())
            {
                return NoContent();
            }

            return BadRequest("Problem deleting the product");

        }


    }
}
