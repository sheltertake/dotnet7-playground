
using Microsoft.AspNetCore.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRateLimiter(opts =>
{
    opts.AddFixedWindowLimiter("TwoPerFive", rateLimiter =>
    {
        rateLimiter.Window = TimeSpan.FromSeconds(5);
        rateLimiter.PermitLimit = 2;
    });
});

builder.Services.AddOutputCache(opts =>
{
    opts.AddPolicy("Fast", cache =>
        cache.Cache().Expire(TimeSpan.FromSeconds(3)));
});
var app = builder.Build();

app.UseOutputCache();
app.UseRateLimiter();

app.MapGet("/", () => "Hello World")
   .RequireRateLimiting("TwoPerFive")
   .CacheOutput();


app.Run();