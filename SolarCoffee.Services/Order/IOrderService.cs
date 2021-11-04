using System.Collections.Generic;
using SolarCoffee.Data.Models;
using SolarCoffee.Services.Product;

namespace SolarCoffee.Services.Order {
	public interface IOrderService {
		List<SalesOrder> GetOrders();
		ServiceResponse<bool> GenerateNewOrder(SalesOrder order);
		ServiceResponse<bool> MakFulfilled(int id);
	}
}