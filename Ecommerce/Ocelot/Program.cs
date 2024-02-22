using WebAPIGateway.Extensions;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Eureka;
using Steeltoe.Discovery.Client;
using Steeltoe.Discovery.Eureka;


var builder = WebApplication.CreateBuilder(args);

var logger = LoggerFactory.Create(config =>
{
    config.AddConsole();
}).CreateLogger("Program");

builder.AddAppAuthentication();

builder.Configuration.AddJsonFile("Ocelot.json", optional: false, reloadOnChange: true);
builder.Services.AddOcelot(builder.Configuration).AddEureka();
//Config Service Discovery
builder.Services.AddDiscoveryClient();
builder.Services.AddHealthChecks();
builder.Services.AddSingleton<IHealthCheckHandler, ScopedEurekaHealthCheckHandler>();

var app = builder.Build();
//app.UseAuthentication();
//app.UseAuthorization();
app.MapGet("/", () => "Hello World!");
//app.UseOcelot();
app.UseOcelot().GetAwaiter().GetResult();
app.Run();






















//using Microsoft.AspNetCore.Authentication.JwtBearer;
//using Microsoft.IdentityModel.Tokens;
//using Ocelot.DependencyInjection;
//using Ocelot.Middleware;
//using System.Text;

//var builder = WebApplication.CreateBuilder(args);

//var settingsSection = builder.Configuration.GetSection("ApiSettings").GetSection("JwtOptions");

//var secret = settingsSection.GetValue<string>("Secret");
//var issuer = settingsSection.GetValue<string>("Issuer");
//var audience = settingsSection.GetValue<string>("Audience");

//var key = Encoding.ASCII.GetBytes(secret);
//builder.Services.AddAuthentication(x =>
//{
//    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
//}).AddJwtBearer(x =>
//{
//    x.TokenValidationParameters = new TokenValidationParameters
//    {
//        ValidateIssuerSigningKey = true,
//        IssuerSigningKey = new SymmetricSecurityKey(key),
//        ValidateIssuer = true,
//        ValidIssuer = issuer,
//        ValidAudience = audience,
//        ValidateAudience = true
//    };
//});

//builder.Configuration.SetBasePath(builder.Environment.ContentRootPath)
//    .AddJsonFile("Ocelot.json",optional: false, reloadOnChange: true)
//    .AddEnvironmentVariables();
//builder.Services.AddOcelot(builder.Configuration);

//var app = builder.Build();
//await app.UseOcelot();

////app.MapGet("/", () => "Hello World!");


//app.Run();
