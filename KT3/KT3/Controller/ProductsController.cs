using KT3.Repository;
using KT3.Models;
using Microsoft.AspNetCore.Mvc;

namespace KT3.Controller
{
    [Route("[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private IUnitOfWork _unitOfWork;
        public ProductsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public IActionResult GetAll() => Ok(_unitOfWork.Products.GetAll);

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var product = _unitOfWork.Products.Get(id);

            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }

        [HttpPost]
        public IActionResult Create(Product product)
        {
            _unitOfWork.Products.Add(product);
            _unitOfWork.Complete();
            return CreatedAtAction(nameof(Get), new {id = product.Id}, product);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, Product product)
        {
            var updatingProduct = _unitOfWork.Products.Get(id);

            if (updatingProduct == null)
            {
                return NotFound();
            }

            updatingProduct.Name = product.Name;
            updatingProduct.Price = product.Price;
            _unitOfWork.Complete();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            _unitOfWork.Products.Delete(id);
            _unitOfWork.Complete();

            return NoContent();
        }
    }
}
