using Microsoft.EntityFrameworkCore;
using CO.Payments.Api.Data.Database;
using CO.Payments.Api.Controllers.ExceptionHandling;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<PaymentsDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("PaymentsDbContext") ?? throw new InvalidOperationException("Connection string 'PaymentsDbContext' not found.")));

// Add services to the container.

builder.Services.AddControllers(options =>
{ 
    options.Filters.Add<HttpResponseExceptionFilter>();
});
builder.Services.AddHealthChecks();
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
app.MapHealthChecks("/api/healthcheck");

app.Run();
public partial class Program { }
