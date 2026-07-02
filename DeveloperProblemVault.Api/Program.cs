using DeveloperProblemVault.Api;
using DeveloperProblemVault.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var connStr = builder.Configuration.GetConnectionString("Default")!;
builder.Services.AddSingleton(new IssueManager(connStr));
builder.Services.AddScoped<IssueService>();

var app = builder.Build();

await app.Services.GetRequiredService<IssueManager>().EnsureCreatedAsync();

if (app.Environment.IsDevelopment())
    app.MapOpenApi();

app.UseExceptionHandler(errApp => errApp.Run(async ctx =>
{
    var ex = ctx.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>()?.Error;
    var (status, reason) = ex is ArgumentException
        ? (400, ex.Message)
        : (500, "An unexpected error occurred.");

    ctx.Response.StatusCode  = status;
    ctx.Response.ContentType = "application/json";
    await ctx.Response.WriteAsJsonAsync(
        new ResponseModel { Stat = 0, Message = MessageConstants.Failed, Reason = reason });
}));

app.UseHttpsRedirection();
app.MapControllers();
app.Run();
