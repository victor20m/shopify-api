using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Models.In;
using Models.Out;
using ShopifySharp;

namespace ShopifyApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerLocalService _customerService;

        public CustomersController(ICustomerLocalService customerService)
        {
            _customerService = customerService;
        }

        [HttpPost("redeem")]
        public async Task<ActionResult<GiftCard>> Post([FromBody] CustomerRedeemModel request)
        {

            if (request is null)
            {
                return BadRequest();
            }
            else
            {
                return await _customerService.Redeem(request);
            }
        }

        [HttpGet("points/{customerId}")]
        public async Task<ActionResult<AvailablePointsModel>> GetPoints([FromRoute] long customerId)
        {

            return await _customerService.GetPoints(customerId);
        }
    }
}
