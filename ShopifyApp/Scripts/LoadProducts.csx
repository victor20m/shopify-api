#r "nuget:ShopifySharp,6.2.0"
#r "nuget:Microsoft.Extensions.Configuration,7.0.0"
#r "nuget:Microsoft.Extensions.Configuration.UserSecrets,7.0.0"
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using ShopifySharp;
using ShopifySharp.Infrastructure;
using System.Net.Http;
using System.Collections.Generic;
using System;

public class Program
{
    static readonly HttpClient client = new HttpClient();

    public async Task Main()
    {
        Console.WriteLine("Script started.");
        try
        {
            //await SubscibeWebhooks();
            await CreateProducts();
        }
        catch (Exception ex)
        {
            Console.WriteLine("\nThe script encountered an error");
            Console.WriteLine(ex.Message);
        }
    }

    static async Task CreateProducts()
    {
        string giphySecret = GetEnvironmentKey("giphy", "api_key");
        HttpResponseMessage response = await client.GetAsync($"https://api.giphy.com/v1/gifs/trending?api_key={giphySecret}&limit=2");
        response.EnsureSuccessStatusCode();
        string responseBody = await response.Content.ReadAsStringAsync();
        var responseData = JObject.Parse(responseBody);
        List<JToken> gifs;
        if (responseData != null && responseData.GetValue("data") != null)
        {
            gifs = responseData.GetValue("data")?.ToList<JToken>();
            if (gifs?.Count > 0)
            {
                List<Product> products = BuildProducts(gifs);
                await CreateShopifyProducts(products);
                Console.WriteLine("Products created successfully");
            }
        }
        else
        {
            throw new Exception("No gifs found");
        }
    }

    static List<Product> BuildProducts(List<JToken> gifs)
    {
        List<Product> products; products = new List<Product>();
        gifs.ForEach(g =>
        {
            var random = new Random();
            List<ProductImage> images = new()
            {
                new ProductImage(){ Src = g["images"]?["fixed_height_still"]?["url"]?.ToString()},
                new ProductImage(){ Src = g["images"]?["fixed_width_small"]?["url"]?.ToString()},
                new ProductImage(){ Src = g["images"]?["fixed_height_small_still"]?["url"]?.ToString() }
            };

            List<ProductVariant> variants = new()
            {
                new ProductVariant
                {
                    Option1 = "Test variant 1",
                    Price = new Random().Next(1, 1000)
                },
                new ProductVariant
                {
                    Option1 = "Test variant 2",
                    Price = new Random().Next(1, 1000)
                },
            };

            products.Add(new()
            {
                Title = g["title"]?.ToString(),
                Vendor = g["user"]?["username"]?.ToString(),
                Images = images,
                Variants = variants
            });
        });
        return products;
    }

    static async Task<List<Product>> CreateShopifyProducts(List<Product> products)
    {
        List<Product> newProducts = new();
        string storeUrl = GetEnvironmentKey("shopify", "store_url");
        string accessToken = GetEnvironmentKey("shopify", "api_secret");
        var productService = new ProductService(storeUrl, accessToken);
        productService.SetExecutionPolicy(new LeakyBucketExecutionPolicy());
        foreach (var product in products)
        {
            try
            {
                var response = await productService.CreateAsync(product);
                newProducts.Add(response);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to create product: {ex.Message}");
            }
        }
        return newProducts;
    }

    static async Task SubscibeWebhooks()
    {
        string apiKey = GetEnvironmentKey("shopify", "api_secret");
        string storeUrl = GetEnvironmentKey("shopify", "store_url");
        string ordersEndpoint = GetEnvironmentKey("api", "orders_endpoint");

        var webbookService = new WebhookService(storeUrl, apiKey);
        webbookService.SetExecutionPolicy(new LeakyBucketExecutionPolicy());
        Webhook hook = new Webhook()
        {
            Address = ordersEndpoint,
            CreatedAt = DateTime.Now,
            Format = "json",
            Topic = "orders/create"
        };

        try
        {
            hook = await webbookService.CreateAsync(hook);
            Console.WriteLine($"Successfully subscribed to {hook.Topic} webhook.");
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to create webhook subscription {ex.Message}");
        }

    }
    
    static string GetEnvironmentKey(string section, string key)
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("secrets.json");

        IConfigurationRoot configuration = builder.Build();
        IConfigurationSection envSection = configuration.GetSection(section);
        if (envSection is null) throw new Exception($"Could not get configuration {section} -> {key}");
        return envSection.GetSection(key).Value;
    }
}
var program = new Program();
program.Main().Wait();