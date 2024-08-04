using Carter;
using Carter.OpenApi;
using KafkaExample;
using KafkaExample.Settings;

var builder = WebApplication.CreateBuilder(args);
var configuration = AppSettings.Configuration();

builder.Services
    .AddWebApi(configuration)
    .AddSwagger();

var app = builder.Build();

app.MapGet("/hello", () => "Hello World!")
    .WithTags("hello")
    .WithName("hello")
    .IncludeInOpenApi();

app.UseSwagger();
app.UseSwaggerUI();
app.MapCarter();
app.Run();