using DeveloperProblemVault.Api;
using DeveloperProblemVault.Api.Config;
using DeveloperProblemVault.Api.Helpers;
using DeveloperProblemVault.Api.Services;
using DeveloperProblemVault.Data;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog((ctx, config) => config.ReadFrom.Configuration(ctx.Configuration));

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddHttpClient();

var connStr = builder.Configuration.GetConnectionString("Default")!;
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connStr, ServerVersion.AutoDetect(connStr)));
builder.Services.AddScoped<IssueManager>();
builder.Services.AddScoped<IssueService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<KeycloakHelper>();
builder.Services.AddSingleton(builder.Configuration.GetSection("Keycloak").Get<KeycloakConfig>()!);

builder.Services.AddStackExchangeRedisCache(options =>
    options.Configuration = builder.Configuration["Redis"]);

builder.Services.AddCors(options =>
    options.AddPolicy("Default", policy =>
        policy.WithOrigins("http://127.0.0.1:5500")
              .AllowAnyMethod()
              .AllowAnyHeader()));

builder.Services.AddAuthentication()
    .AddJwtBearer(options =>
    {
        options.Authority = $"{builder.Configuration["Keycloak:AuthServerUrl"]}/realms/{builder.Configuration["Keycloak:Realm"]}";
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateAudience = false
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
    await scope.ServiceProvider.GetRequiredService<AppDbContext>().Database.EnsureCreatedAsync();

if (app.Environment.IsDevelopment())
    app.MapOpenApi();

app.UseExceptionHandler(errApp => errApp.Run(async ctx =>
{
    var ex = ctx.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>()?.Error;
    var logger = ctx.RequestServices.GetRequiredService<ILogger<Program>>();

    var (status, reason) = ex switch
    {
        ArgumentException       => (400, ex.Message),
        UnauthorizedAccessException => (401, ex.Message),
        KeyNotFoundException    => (404, ex.Message),
        _                       => (500, "An unexpected error occurred.")
    };

    logger.LogError(ex, "Unhandled exception - Status: {Status}, Reason: {Reason}", status, reason);

    ctx.Response.StatusCode  = status;
    ctx.Response.ContentType = "application/json";
    await ctx.Response.WriteAsJsonAsync(
        new ResponseModel { Stat = 0, Message = MessageConstants.Failed, Reason = reason });
}));

app.UseHttpsRedirection();
app.UseCors("Default");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
