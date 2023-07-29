using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Security.Cryptography;

namespace ShopifyApp.Filters
{
    public class WebhookAuthFilter : IAsyncAuthorizationFilter
    {
        private readonly IConfiguration _config;
        public WebhookAuthFilter(IConfiguration config)
        {
            _config = config;
        }
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            string shopifySecretKey = _config["shopify:client_secret"];
            var requestBody = context.HttpContext.Request.Body;
            var requestHeaders = context.HttpContext.Request.Headers;


            //TODO: AuthorizationService.IsAuthenticWebhook -> can't make this work with aspnet core following docs, need to implement another way
            try
            {
                bool isValidRequest = await IsAuthenticWebhook(requestHeaders, requestBody, shopifySecretKey);

                if (!isValidRequest)
                {
                    context.Result = GetContentResult(403, "Access forbidden, request is not coming from Shopify.");
                }
            }
            catch (Exception)
            {
                context.Result = GetContentResult(500, "Server error authenticating webhook request.");
            }

        }
        public async Task<bool> IsAuthenticWebhook(IHeaderDictionary requestHeaders, Stream requestBodyStream, string shopifySecretKey)
        {

            using var reader = new StreamReader(requestBodyStream);
            var requestBody = await reader.ReadToEndAsync();

            if (!requestHeaders.TryGetValue("X-Shopify-Hmac-SHA256", out var hmacHeaderValues) || hmacHeaderValues.Count == 0)
            {
                return false;
            }

            string hmacHeader = hmacHeaderValues.First();
            using HMACSHA256 hmac = new HMACSHA256(Encoding.UTF8.GetBytes(shopifySecretKey));
            string hash = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(requestBody)));
            return hash == hmacHeader;
        }

        private static ContentResult GetContentResult(int code, string message)
        {
            return new ContentResult()
            {
                StatusCode = code,
                Content = System.Text.Json.JsonSerializer.Serialize(new
                {
                    code,
                    error = message
                }),
                ContentType = "application/json"
            };
        }
    }
}
