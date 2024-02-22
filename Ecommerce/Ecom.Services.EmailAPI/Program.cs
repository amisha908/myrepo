using Ecom.Services.EmailAPI.Data;
using Ecom.Services.EmailAPI.Messaging;
using Ecom.Services.EmailAPI.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var logger = LoggerFactory.Create(config =>
{
    config.AddConsole();
}).CreateLogger("Program");
logger.LogInformation($"My connection String is: {builder.Configuration.GetValue<string>("ConnectionStrings:constr")}");
logger.LogInformation($"My RabbitMQ connection String is: {builder.Configuration.GetValue<string>("ConnectionStrings:RabbitMQConnection")}");


builder.Services.AddDbContext<AppDbContext>(option =>
{
    option.UseSqlServer(builder.Configuration.GetConnectionString("constr"));
});

var optionBuiler = new DbContextOptionsBuilder<AppDbContext>();
optionBuiler.UseSqlServer(builder.Configuration.GetConnectionString("constr"));
builder.Services.AddSingleton(new EmailService(optionBuiler.Options));

builder.Services.AddHostedService<RabbitMQAuthConsumer>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
ApplyMigration();

app.Run();
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