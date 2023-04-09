using Microsoft.EntityFrameworkCore;
using CO.Payments.Api.Data.Database;
using CO.Payments.Api.Controllers.ExceptionHandling;
using CO.Payments.Api.Services;
using Swashbuckle.AspNetCore.Filters;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<PaymentsDbContext>(options =>
    options.UseSqlite($"Data Source=dev"));

// Add services to the container.

builder.Services.AddControllers(options =>
{ 
    options.Filters.Add<HttpResponseExceptionFilter>();
});
builder.Services.AddHealthChecks();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options => {
    options.ExampleFilters();
});
builder.Services.AddSwaggerExamplesFromAssemblies(Assembly.GetEntryAssembly());

builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddSingleton<IBankService, CKOBankService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<PaymentsDbContext>();
        db.Database.EnsureCreated();
        await db.SeedTestData();
    }
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/api/healthcheck");

app.Run();
public partial class Program { }
