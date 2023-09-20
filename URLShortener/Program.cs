using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;
using URLShortener.Extensions;
using URLShortener.Models;
using URLShortener.ServiceContracts;
using URLShortener.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(policyBuilder =>
    policyBuilder.AddDefaultPolicy(policy =>
        policy.WithOrigins("*").AllowAnyHeader().AllowAnyMethod())
);
builder.Services.ConnectRedis(builder.Configuration);
builder.Services.AddSingleton<IUrlShortenerService, UrlShortenerService>();

var app = builder.Build();

app.UseCors();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

IResult Result(ServiceResponse serviceResponse, HttpContext httpContext)
{
    if (serviceResponse.Success)
    {
        serviceResponse.ShortenedUrl = serviceResponse.UniqueCode!.UrlFormat(httpContext);
        return Results.Ok(serviceResponse);
    }

    return Results.BadRequest(serviceResponse);
}

app.MapPost("/UrlShorter", async (string originalUrl,
    IUrlShortenerService urlShortenerService,
    HttpContext httpContext) =>
{
    if (String.IsNullOrWhiteSpace(originalUrl))
    {
        return Results.BadRequest($"Url cannot be empty or null");
    }

    var serviceResponse = await urlShortenerService.Shorter(originalUrl);

    return Result(serviceResponse, httpContext);
});

app.MapPost("/CustomUrl", async (string originalUrl,
    string customCode,
    IUrlShortenerService urlShortenerService,
    HttpContext httpContext) =>
{
    if (String.IsNullOrWhiteSpace(originalUrl) || String.IsNullOrWhiteSpace(customCode))
    {
        return Results.BadRequest(ServiceResponse.FailedResponse($"Parameters cannot be empty or null"));
    }

    var serviceResponse = await urlShortenerService.CustomUrl(originalUrl, customCode);

    return Result(serviceResponse, httpContext);

});

app.MapFallback("/{shortCode}", async (string shortCode,IUrlShortenerService urlShortenerService) =>
{
    if (String.IsNullOrWhiteSpace(shortCode))
    {
        return Results.BadRequest(ServiceResponse.FailedResponse($"Short code cannot be empyt or null"));
    }

    string? originalUrl = await urlShortenerService.GetOriginalUrl(shortCode);

    if(originalUrl is null)
    {
        return Results.NotFound(ServiceResponse.FailedResponse($"No url found for this shortCode({shortCode})"));
    }

    return Results.Redirect(originalUrl);

});
app.UseHttpsRedirection();
app.Run();

