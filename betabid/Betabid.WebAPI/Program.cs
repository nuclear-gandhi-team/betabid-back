using Betabid.Application.Helpers;
using Betabid.Application.Helpers.Options;
using Betabid.Application.Profiles;
using Betabid.Application.Services;
using Betabid.Application.Validators;
using Betabid.Domain.Entities;
using betabid.Extensions;
using betabid.Middleware;
using Betabid.Persistence.Context;
using Betabid.Persistence.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddAuthConfigurations(builder.Configuration);

builder.Services.AddControllers();

builder.Services.AddValidators();

builder.Services.AddScopedServices();

builder.Services.AddScopedRepositories();

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("JWT"));

builder.Services.AddScoped<ITimeProvider, TimeProvider>();

builder.Services.AddRouting(options => options.LowercaseUrls = true);

builder.Services.AddAutoMapper(typeof(ApplicationProfile));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

string connectionString;
if (builder.Environment.IsDevelopment())
{
    connectionString = builder.Configuration.GetConnectionString("LocalConnStr")
        ?? throw new NullReferenceException("Connection string to local DB os not set.");
}
else
{
    connectionString = builder.Configuration.GetConnectionString("AzureConnStr")
                       ?? throw new NullReferenceException("Connection string to Azure DB os not set.");
}
    
builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<DataContext>()
    .AddDefaultTokenProviders();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var dbContext = services.GetRequiredService<DataContext>();
        dbContext.Database.Migrate();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"{ex.Message} - An error occurred while applying the database migrations.");
    }
}

app.UseMiddleware<ExceptionHandlerMiddleware>();

app.UseCors(corsPolicyBuilder => corsPolicyBuilder
    .AllowAnyMethod()
    .AllowAnyHeader()
    .SetIsOriginAllowed(_ => true)
    .WithExposedHeaders("Content-Disposition")
    .WithOrigins("http://localhost:8080"));


// Configure the HTTP request pipeline.
/*if (app.Environment.IsDevelopment())
{*/
    app.UseSwagger();
    app.UseSwaggerUI();
/*}*/

app.UseHttpsRedirection();

app.UseAuthorization();
app.UseAuthentication();

app.MapControllers();

app.Run();