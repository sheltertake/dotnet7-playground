
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

var app = builder.Build();

app.UseRateLimiter();
app.MapGet("/", () => "Hello World")
   .RequireRateLimiting("TwoPerFive");


app.Run();