using Microsoft.AspNetCore.Mvc;
using MinimalAPI.EndpointFilters;
using MinimalAPI.Models;
using System.ComponentModel.DataAnnotations;
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
                //context.Response.WriteAsync(JsonSerializer.Serialize(products));
                return Results.Ok(products);
            });

           group.MapGet("/{id:int}", async (HttpContext context, int id) => {
                Product? product = products.FirstOrDefault(temp => temp.Id == id);
                if (product == null)
                {
                   //context.Response.StatusCode = 400;//Bad Request
                   //await context.Response.WriteAsync("Incorrect Product ID");
                   return Results.BadRequest(new {error = "Incorrect Product ID"});
                }
               //await context.Response.WriteAsync(JsonSerializer.Serialize(product));
               return Results.Ok(product);
            });

            group.MapPost("/", async (HttpContext context, Product product) =>
            {
                products.Add(product);
                //await context.Response.WriteAsync("Product Added");
                return Results.Ok(new { message = "Product Added" });
            })
                .AddEndpointFilter<CustomEndpointFilter>()
                .AddEndpointFilter(async(EndpointFilterInvocationContext context,EndpointFilterDelegate next) => {
                    var product = context.Arguments.OfType<Product>().FirstOrDefault();

                    if (product == null)
                    {
                        return Results.BadRequest("Product details are not in the request");
                    }
                    var validationContext = new ValidationContext(product);
                    List<ValidationResult> errors = new List<ValidationResult>();
                    bool isValid = Validator.TryValidateObject(product, validationContext,errors,true);

                    if (!isValid)
                    {
                        return Results.BadRequest(errors.FirstOrDefault()?.ErrorMessage);
                    }
                    var result = await next(context);
                    return result;
                });

            //Update

            group.MapPut("/{id}",async (HttpContext context,int id, [FromBody] Product product)=> {
                Product? productCollection = products.FirstOrDefault(temp=> temp.Id == id);
                if (productCollection == null)
                {
                    //context.Response.StatusCode = 400;

                    //await context.Response.WriteAsync("Product Not found");
                    //return;
                    return Results.Ok(new { error = "Product Not found" });

                }
                productCollection.ProductName = product.ProductName;
                //await context.Response.WriteAsync("Product Updated");
                return Results.Ok(new { message = "Product Updated"});
            });

            group.MapDelete("/{id}", async (HttpContext context, int id) => {
                Product? productCollection = products.FirstOrDefault(temp => temp.Id == id);
                if (productCollection == null)
                {
                    //context.Response.StatusCode = 400;

                    //await context.Response.WriteAsync("Product Not found");
                    //return;
                    //return Results.Ok(new { error = "Product Not found"});
                    return Results.ValidationProblem(new Dictionary<string, string[]> { { "id", new string[] {"Incorrect Product ID" } } });

                }
                products.Remove(productCollection);
                //await context.Response.WriteAsync("Product Deleted");
                return Results.Ok(new { message = "Product Deleted" });
            });
            return group;
        }

    }
}
