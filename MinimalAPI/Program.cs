using MinimalAPI.Models;
using MinimalAPI.RoutesGroup;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();






var mapGroup = app.MapGroup("/products").ProductAPI();


app.Run();
