using System.ComponentModel.DataAnnotations;

namespace CO.Payments.Api.Data.DbModels
{
    public class MerchantPaymentProfile
    {
        [Key]
        public long MerchantId { get; set; }
        public string MerchantBankReference { get; set; }
    }
}
