using DeveloperProblemVault.Api;
using DeveloperProblemVault.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var connStr = builder.Configuration.GetConnectionString("Default")!;
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connStr, ServerVersion.AutoDetect(connStr)));
builder.Services.AddScoped<IssueManager>();
builder.Services.AddScoped<IssueService>();

builder.Services.AddCors(options =>
    options.AddPolicy("Default", policy =>
        policy.WithOrigins("http://127.0.0.1:5500")
              .AllowAnyMethod()
              .AllowAnyHeader()));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
    await scope.ServiceProvider.GetRequiredService<AppDbContext>().Database.EnsureCreatedAsync();

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
app.UseCors("Default");
app.MapControllers();
app.Run();
