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
        string giphySecret = GetEnvironmentKey("giphy", "api_key");
        Console.WriteLine("Script started.");
        try
        {
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
                Console.WriteLine("No gifs found");
            }
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine("\nError retrieving gifs");
            Console.WriteLine(e.Message);
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
                Console.WriteLine(response);
                newProducts.Add(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to create product: {ex.Message}");
            }
        }
        return newProducts;
    }

    static string GetEnvironmentKey(string section, string key)
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("secrets.json");

        IConfigurationRoot configuration = builder.Build();
        IConfigurationSection envSection = configuration.GetSection(section);
        return envSection.GetSection(key).Value;
    }
}
var program = new Program();
program.Main().Wait();