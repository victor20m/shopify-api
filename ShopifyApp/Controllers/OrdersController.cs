using Microsoft.AspNetCore.Mvc;
using ShopifyApp.Filters;
using ShopifySharp;

namespace ShopifyApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        // GET: api/<OrdersController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<OrdersController>/5
        [HttpGet("{id}")]
        [ServiceFilter(typeof(WebhookAuthFilter))]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<OrdersController>
        [HttpPost]
        [ServiceFilter(typeof(WebhookAuthFilter))]
        public async Task<ActionResult<string>> Post([FromBody] Order order)
        {
            return "test";
        }
    }
}
