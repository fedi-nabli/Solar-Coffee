using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SolarCoffee.Services.Product;
using SolarCoffee.Web.Serialization;

namespace SolarCoffee.Web.Controllers {
	[ApiController]
	public class ProductController : ControllerBase {
		private readonly ILogger<ProductController> _logger;
		private readonly IProductService _productService;

		public ProductController(ILogger<ProductController> logger, IProductService productService) {
			_logger = logger;
			_productService = productService;
		}

		[HttpGet("/api/products")]
		public ActionResult GetProducts() {
			_logger.LogInformation("Getting all Products");
			var products = _productService.GetAllProducts();
			var productViewModels = products
				.Select(ProductMapper.SerializeProductModel);
			
			return Ok(productViewModels);
		}
	}
}