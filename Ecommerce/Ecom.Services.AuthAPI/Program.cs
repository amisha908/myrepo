using Ecom.Services.AuthAPI.Data;
using Ecom.Services.AuthAPI.Models;
using Ecom.Services.AuthAPI.RabbitMQSender;
using Ecom.Services.AuthAPI.Service;
using Ecom.Services.AuthAPI.Service.IService;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Steeltoe.Discovery.Client;
using Steeltoe.Discovery.Eureka;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//Docker Container Logs
var logger = LoggerFactory.Create(config =>
{
    config.AddConsole();
}).CreateLogger("Program");
logger.LogInformation($"My connection String is: {builder.Configuration.GetValue<string>("ConnectionStrings:constr")}");
logger.LogInformation($"My RabbitMQ connection String is: {builder.Configuration.GetValue<string>("ConnectionStrings:RabbitMQConnection")}");

//Add DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("constr"));
});

builder.Services.AddServiceDiscovery(option => option.UseEureka());

//Config Service Discovery
builder.Services.AddDiscoveryClient();
builder.Services.AddHealthChecks();
builder.Services.AddSingleton<IHealthCheckHandler, ScopedEurekaHealthCheckHandler>();

//Configure JWT
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("ApiSettings:JwtOptions"));

//Configure Identity Tables
builder.Services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();


builder.Services.AddControllers();

//Configure Services
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IRabbitMQAuthMessageSender, RabbitMQAuthMessageSender>();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
    options.AddPolicy("User", policy => policy.RequireRole("User"));
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.UseHealthChecks("/info");
ApplyMigration();
app.Run();

//Function to automatically apply Migrations to Db
void ApplyMigration()
{
    using (var scope = app.Services.CreateScope())
    {
        var _db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        if (_db.Database.GetPendingMigrations().Count() > 0)
        {
            _db.Database.Migrate();
        }
    }
}
