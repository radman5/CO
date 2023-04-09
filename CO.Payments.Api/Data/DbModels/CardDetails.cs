using System.ComponentModel.DataAnnotations;
using CO.Payments.Api.Data.Lookups;

namespace CO.Payments.Api.Data.DbModels;

public class CardDetails
{
    [Key]
    public string Token { get; private set; }
    public string CardNumber { get; private set; }
    public string CardHolder { get; private set; }
    public string Expiry { get; private set; }
    public string Cvv { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UsedAt { get; private set; }
    public CardDetailsStatus Status { get; private set; }
    public long MerchantId { get; private set; }

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

    internal void Use()
    {
        Status = CardDetailsStatus.Used;
        UsedAt = DateTime.UtcNow;
    }
}
