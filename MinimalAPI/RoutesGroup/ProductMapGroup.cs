using Microsoft.AspNetCore.Mvc;
using MinimalAPI.Models;
using System.Text.Json;

namespace MinimalAPI.RoutesGroup
{
    public static class ProductMapGroup
    {
       private static  List<Product> products = new List<Product>() 
       {
        new Product() {Id = 1, ProductName = "Smart Phone" },
        new Product() {Id = 2, ProductName = "Smart Tv" }
       };

        public static RouteGroupBuilder ProductAPI(this RouteGroupBuilder group)
        {
            group.MapGet("/", async (HttpContext context) => {
                context.Response.WriteAsync(JsonSerializer.Serialize(products));
            });

           group.MapGet("/{id:int}", async (HttpContext context, int id) => {
                Product? product = products.FirstOrDefault(temp => temp.Id == id);
                if (product == null)
                {
                    context.Response.StatusCode = 400;//Bad Request
                    await context.Response.WriteAsync("Incorrect Product ID");
                    return;
                }
                await context.Response.WriteAsync(JsonSerializer.Serialize(product));
            });

            group.MapPost("/", async (HttpContext context, Product product) => {
                products.Add(product);
                await context.Response.WriteAsync("Product Added");
            });

            //Update

            group.MapPut("/{id}",async (HttpContext context,int id, [FromBody] Product product)=> {
                Product? productCollection = products.FirstOrDefault(temp=> temp.Id == id);
                if (productCollection == null)
                {
                    context.Response.StatusCode = 400;

                    await context.Response.WriteAsync("Product Not found");
                    return;

                }
                productCollection.ProductName = product.ProductName;
                await context.Response.WriteAsync("Product Updated");
            });

            group.MapDelete("/{id}", async (HttpContext context, int id) => {
                Product? productCollection = products.FirstOrDefault(temp => temp.Id == id);
                if (productCollection == null)
                {
                    context.Response.StatusCode = 400;

                    await context.Response.WriteAsync("Product Not found");
                    return;

                }
                products.Remove(productCollection);
                await context.Response.WriteAsync("Product Deleted");
            });
            return group;
        }

    }
}
