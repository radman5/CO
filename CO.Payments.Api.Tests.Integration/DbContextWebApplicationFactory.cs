using CO.Payments.Api.Data.Database;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CO.Payments.Api.Tests.Integration
{
    public class DbContextWebApplicationFactory<TProgram>
        : WebApplicationFactory<TProgram> where TProgram : class
    {
        private const string ConnectionString = "DataSource=:memory:";
        private readonly SqliteConnection _connection;
        private bool _disposed;

        public DbContextWebApplicationFactory()
        {
            _connection = new SqliteConnection(ConnectionString);
            _connection.Open();
            var options = new DbContextOptionsBuilder<PaymentsDbContext>()
                    .UseSqlite(_connection)
                    .Options;
            var dbContext = CreateContext();
            dbContext.Database.EnsureCreated();
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                var dbContextDescriptor = services.SingleOrDefault(
                    d => d.ServiceType ==
                        typeof(DbContextOptions<PaymentsDbContext>));

                services.Remove(dbContextDescriptor);

                var dbConnectionDescriptor = services.SingleOrDefault(
                    d => d.ServiceType ==
                        typeof(PaymentsDbContext));

                services.Remove(dbConnectionDescriptor);

                services.AddDbContext<PaymentsDbContext>((container, options) =>
                {
                    options.UseSqlite(_connection);
                });
            });

            builder.UseEnvironment("Development");
        }

        public PaymentsDbContext CreateContext()
        => new PaymentsDbContext(
            new DbContextOptionsBuilder<PaymentsDbContext>()
                .UseSqlite(_connection)
                .Options);

        protected override void Dispose(bool disposing)
        {
            if (!_disposed &&
                disposing)
            {
                _connection.Close();
            }

            _disposed = true;
            base.Dispose(disposing);
        }
    }
}
