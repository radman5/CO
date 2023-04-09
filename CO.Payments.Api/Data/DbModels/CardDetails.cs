using System.ComponentModel.DataAnnotations;
using CO.Payments.Api.Data.Lookups;

namespace CO.Payments.Api.Data.DbModels;

public class CardDetails
{
    [Key]
    public string Token { get; set; }
    public string CardNumber { get; set; }
    public string CardHolder { get; set; }
    public string Expiry { get; set; }
    public string Cvv { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UsedAt { get; set; }
    public CardDetailsStatus Status { get; set; }
    public long MerchantId { get; set; }

    public static CardDetails Create(string cardNumber, string cardHolderName, string expiryDate, string cvv, long merchantId)
    {
        return new CardDetails
        {
            Token = Guid.NewGuid().ToString(),
            CardNumber = cardNumber,
            CardHolder = cardHolderName,
            Expiry = expiryDate,
            Cvv = cvv,
            CreatedAt = DateTime.UtcNow,
            Status = CardDetailsStatus.Pending,
            MerchantId= merchantId
        };
    }

    public void Use()
    {
        Status = CardDetailsStatus.Used;
        UsedAt = DateTime.UtcNow;
    }
}
