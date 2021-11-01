using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SolarCoffee.Data;
using SolarCoffee.Data.Models;
using SolarCoffee.Services.Product.Inventory;

namespace SolarCoffee.Services.Product.Order {
	public class OrderService : IOrderService {
		private readonly SolarDbContext _db;
		private readonly ILogger<OrderService> _logger;
		private readonly InventoryService _inventoryService;
		private readonly ProductService _productService;

		public OrderService(
			SolarDbContext dbContext,
			ILogger<OrderService> logger,
			InventoryService inventoryService,
			ProductService productService) {
			_db = dbContext;
			_logger = logger;
			_inventoryService = inventoryService;
			_productService = productService;
		}

		public List<SalesOrder> GetOrders() {
			return _db.SalesOrders
				.Include(so => so.Customer)
					.ThenInclude(customer => customer.PrimaryAddress)
				.Include(so => so.SalesOrderItems)
					.ThenInclude(item => item.Product)
				.ToList();
		}

		public ServiceResponse<bool> GenerateInvoiceForOrder(SalesOrder order) {
			var now = DateTime.UtcNow;
			
			_logger.LogInformation("Generating new order");
			
			foreach (var item in order.SalesOrderItems) {
				item.Product = _productService.GetProductById(item.Product.Id);
				var inventoryId = _inventoryService.GetByProductId(item.Product.Id).Id;
				_inventoryService.UpdateUnitsAvailable(inventoryId, -item.Quantity);
			}
			
			try {
				_db.SalesOrders.Add(order);
				_db.SaveChanges();

				return new ServiceResponse<bool> {
					IsSuccess = true,
					Data = true,
					Message = "Open order created",
					Time = now
				};
			}
			catch (Exception e) {
				return new ServiceResponse<bool> {
					IsSuccess = false,
					Data = false,
					Message = e.StackTrace,
					Time = now
				};
			}
		}

		public ServiceResponse<bool> MarkFulfilled(int id) {
			var now = DateTime.UtcNow;
			var order = _db.SalesOrders.Find(id);
			order.UpdatedOn = now;
			order.IsPaid = true;

			try {
				_db.SalesOrders.Update(order);
				_db.SaveChanges();

				return new ServiceResponse<bool> {
					IsSuccess = true,
					Data = true,
					Message = $"Order {order.Id} closed: Invoice paid in full.",
					Time = now
				};
			}
			catch (Exception e) {
				return new ServiceResponse<bool> {
					IsSuccess = false,
					Data = false,
					Message = e.StackTrace,
					Time = now
				};
			}
		}
	}
}