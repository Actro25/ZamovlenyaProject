using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SharedModels;
using System.Text.Json;
namespace OrderService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _userServiceUrl = "https://localhost:7134/api/user";
        private readonly string _productServiceUrl = "https://localhost:7178/api/product";

        //private static readonly List<Order> Orders = new List<Order>();
        private static readonly List<Order> Orders = new List<Order>
        {
         new Order { Id = 1, UserId = 1, ProductId = 1, Quantity = 1 },
         new Order { Id = 2, UserId = 2, ProductId = 2, Quantity = 2 }
        };
        public OrderController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpPost]
        public async Task<ActionResult<Order>> CreateOrder([FromBody] Order order)
        {
            var user = await GetUserById(order.UserId);
            if (user == null)
                return BadRequest("User not found");

            var product = await GetProductById(order.ProductId);
            if (product == null)
                return BadRequest("Product not found");


            order.Id = Orders.Max(p => p.Id) + 1;
            Orders.Add(order);
            return Ok(order);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Order>> UpdateOrder(int id, [FromBody] Order updatedOrder)
        {
            var existingOrder = Orders.FirstOrDefault(o => o.Id == id);
            if (existingOrder == null)
                return NotFound("Order not found");

            var user = await GetUserById(updatedOrder.UserId);
            if (user == null)
                return BadRequest("User not found");

            var product = await GetProductById(updatedOrder.ProductId);
            if (product == null)
                return BadRequest("Product not found");

            // Обновление полей существующего заказа
            existingOrder.UserId = updatedOrder.UserId;
            existingOrder.ProductId = updatedOrder.ProductId;
            existingOrder.Quantity = updatedOrder.Quantity;

            return Ok(existingOrder);
        }

        [HttpDelete("{id}")]
        public ActionResult DeleteOrder(int id)
        {
            var order = Orders.FirstOrDefault(o => o.Id == id);
            if (order == null)
                return NotFound("Order not found");

            Orders.Remove(order);
            return Ok($"Order with ID {id} deleted.");
        }

        private async Task<User?> GetUserById(int userId)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync($"{_userServiceUrl}/{userId}");
            if (!response.IsSuccessStatusCode)
                return null;

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<User>(json);
        }

        private async Task<Product?> GetProductById(int productId)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync($"{_productServiceUrl}/{productId}");
            if (!response.IsSuccessStatusCode)
                return null;

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<Product>(json);
        }

        [HttpGet("{id}")]
        public ActionResult<Order> GetOrderById(int id)
        {
            var order = Orders.FirstOrDefault(o => o.Id == id);
            if (order == null)
                return NotFound("Order not found");

            return Ok(order);
        }

        [HttpGet]
        public ActionResult<List<Order>> GetAllOrders()
        {
            return Ok(Orders);
        }
    }

}
