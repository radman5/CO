using CO.Payments.Api.Data.Database;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;

namespace CO.Payments.Api.Tests.Integration
{
    public class DbContextWebApplicationFactory<TProgram>
        : WebApplicationFactory<TProgram> where TProgram : class
    {
        private const string ConnectionString = @"Server=(localdb)\mssqllocaldb;Database=paymentsdb;Trusted_Connection=True";

        public PaymentsDbContext CreateContext()
        => new PaymentsDbContext(
            new DbContextOptionsBuilder<PaymentsDbContext>()
                .UseSqlite(ConnectionString)
                .Options);
    }
}
