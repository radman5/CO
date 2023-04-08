using CO.Payments.Api.Data.Database;
using CO.Payments.Api.Data.DbModels;
using CO.Payments.Api.Data.DTOs;
using FluentAssertions;
using System;
using System.Drawing.Text;
using System.Net;
using System.Net.Http.Json;

namespace CO.Payments.Api.Tests.Integration.PaymentTokenController
{
    public class CreatePaymentTokenTests
        : IClassFixture<DbContextWebApplicationFactory<Program>>
    {
        private readonly DbContextWebApplicationFactory<Program> _factory;
        private readonly PaymentsDbContext _db;

        private const string MerchantIdHeader = "MerchantId";

        public CreatePaymentTokenTests(DbContextWebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _db = _factory.CreateContext();
        }

        [Fact]
        public async Task ValidRequest_ShouldReturnToken_ShouldCreateCardDetails()
        {
            // Arrange
            var createPaymentTokenRequest = GenerateCreatePaymentTokenRequest();

            var merchant = new MerchantPaymentProfile
            {
                MerchantId = 123,
                MerchantBankReference = Guid.NewGuid().ToString()
            };

            await _db.Merchants.AddAsync(merchant);
            await _db.SaveChangesAsync();
            var client = _factory.CreateClient();
            client.DefaultRequestHeaders.Add(MerchantIdHeader, merchant.MerchantId.ToString());

            // Pre Assert
            _db.CardDetails.Any(
                cardDetails => cardDetails.CardNumber == createPaymentTokenRequest.CardNumber)
                .Should().BeFalse();

            // Act
            var response = await client.PostAsJsonAsync("/api/paymenttoken", createPaymentTokenRequest);

            // Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            var paymentToken = await response.Content.ReadFromJsonAsync<CreatePaymentTokenResponse>();
            paymentToken.Should().NotBeNull();
            _db.CardDetails.Any(
                cardDetails => cardDetails.CardNumber == createPaymentTokenRequest.CardNumber)
                .Should().BeTrue();
        }

        [Fact]
        public async Task MissingMerchantIdHeader_ShouldReturnBadRequest()
        {
            // Arrange
            var createPaymentTokenRequest = GenerateCreatePaymentTokenRequest();
            var client = _factory.CreateClient();

            // Pre Assert
            _db.CardDetails.Any(
                cardDetails => cardDetails.CardNumber == createPaymentTokenRequest.CardNumber)
                .Should().BeFalse();

            // Act
            var response = await client.PostAsJsonAsync("/api/paymenttoken", createPaymentTokenRequest);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task MerchantDoesntExist_ShouldReturnBadRequest()
        {
            // Arrange
            var createPaymentTokenRequest = GenerateCreatePaymentTokenRequest();
            var client = _factory.CreateClient();
            client.DefaultRequestHeaders.Add(MerchantIdHeader, "doesntexist");

            // Pre Assert
            _db.CardDetails.Any(
                cardDetails => cardDetails.CardNumber == createPaymentTokenRequest.CardNumber)
                .Should().BeFalse();

            // Act
            var response = await client.PostAsJsonAsync("/api/paymenttoken", createPaymentTokenRequest);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task CreateCardDetails_WithNullRequst_ShouldReturnBadReq()
        {
            // Arrange
            CreatePaymentTokenRequest createPaymentTokenRequest = null;
            var client = _factory.CreateClient();

            // Act
            var response = await client.PostAsJsonAsync("/api/paymenttoken", createPaymentTokenRequest);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        private CreatePaymentTokenRequest GenerateCreatePaymentTokenRequest()
        {
            var generatedCardNumber = GenerateRandomNumberAsString(12);
            var generatedCvv = GenerateRandomNumberAsString(3);
            return new CreatePaymentTokenRequest
            {
                CardNumber = generatedCardNumber,
                CardHolder = "M Bean",
                Expiry = "01/26",
                Cvv = generatedCardNumber
            };

            string GenerateRandomNumberAsString(int length)
            {
                var random = new Random();
                const string chars = "0123456789";
                return new string(Enumerable.Repeat(chars, length)
                    .Select(s => s[random.Next(s.Length)]).ToArray());
            }
        }
    }
}
