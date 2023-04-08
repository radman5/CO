using Microsoft.AspNetCore.Mvc.Testing;

namespace CO.Payments.Api.Tests.Integration
{
    public class DbContextWebApplicationFactory<TProgram>
        : WebApplicationFactory<TProgram> where TProgram : class
    {
        private const string ConnectionString = @"Server=(localdb)\mssqllocaldb;Database=paymentsdb;Trusted_Connection=True";

        public PaymentsDbContext CreateContext()
        => new PaymentsDbContext(
            new DbContextOptionsBuilder<PaymentsDbContext>()
                .UseSqlServer(ConnectionString)
                .Options);
    }
}
