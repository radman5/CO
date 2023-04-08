using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CO.Payments.Api.Data.DbModels;

namespace CO.Payments.Api.Data.Database
{
    public class PaymentsDbContext : DbContext
    {
        public PaymentsDbContext(DbContextOptions<PaymentsDbContext> options)
            : base(options)
        {
        }

        public DbSet<CardDetails> CardDetails { get; set; } = default!;
    }
}
