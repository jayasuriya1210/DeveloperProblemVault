using DeveloperProblemVault.Api;
using DeveloperProblemVault.Api.Config;
using DeveloperProblemVault.Api.Helpers;
using DeveloperProblemVault.Api.Services;
using DeveloperProblemVault.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

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

builder.Services.AddCors(options =>
    options.AddPolicy("Default", policy =>
        policy.WithOrigins("http://127.0.0.1:5500")
              .AllowAnyMethod()
              .AllowAnyHeader()));

// --- Keycloak JWT validation ---
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
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
