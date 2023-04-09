using System.ComponentModel.DataAnnotations;

namespace CO.Payments.Api.Data.DbModels;

public class MerchantPaymentProfile
{
    [Key]
    public long MerchantId { get; set; }
    public string MerchantBankReference { get; set; }

    public static MerchantPaymentProfile Create(string merchantBankReference)
    {
        return new MerchantPaymentProfile
        {
            MerchantBankReference = merchantBankReference
        };
    }
}
