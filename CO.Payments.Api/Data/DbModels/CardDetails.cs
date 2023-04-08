using System.ComponentModel.DataAnnotations;
using CO.Payments.Api.Data.Lookups;

namespace CO.Payments.Api.Data.DbModels
{
    public class CardDetails
    {
        [Key]
        public string Token { get; set; }
        public string CardNumber { get; set; }
        public string CardHolderName { get; set; }
        public string ExpiryDate { get; set; }
        public string Cvv { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UsedAt { get; set; }
        public CardDetailsStatus Status { get; set; }

        public static CardDetails Create(string cardNumber, string cardHolderName, string expiryDate, string cvv)
        {
            return new CardDetails
            {
                Token = Guid.NewGuid().ToString(),
                CardNumber = cardNumber,
                CardHolderName = cardHolderName,
                ExpiryDate = expiryDate,
                Cvv = cvv,
                CreatedAt = DateTime.UtcNow,
                Status = CardDetailsStatus.Pending
            };
        }
    }
}
