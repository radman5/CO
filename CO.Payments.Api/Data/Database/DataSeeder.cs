using CO.Payments.Api.Data.DbModels;

namespace CO.Payments.Api.Data.Database
{
    public static class DataSeeder
    {
        public static async Task SeedTestData(this PaymentsDbContext context)
        {
            var testMerchant = MerchantPaymentProfile.Create("bank_ref_1");
            await context.Merchants.AddAsync(testMerchant);
            await context.SaveChangesAsync();
        }
    }
}
