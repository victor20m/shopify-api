using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using ShopifyApp.Filters;
using ShopifySharp;

namespace ShopifyApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderLocalService _orderService;

        public OrdersController(IOrderLocalService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost]
        [ServiceFilter(typeof(WebhookAuthFilter))]
        public async Task<ActionResult> Post([FromBody] dynamic request)
        {
            string requestText = request.ToString();

            Order order = Newtonsoft.Json.JsonConvert.DeserializeObject<Order>(requestText);
            if (order is null || order.Customer is null)
            {
                return BadRequest();
            }
            else
            {
                await _orderService.Create(order);
            }
            return Ok();
        }
    }
}
