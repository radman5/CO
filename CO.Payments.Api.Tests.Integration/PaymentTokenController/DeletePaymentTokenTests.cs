using AutoFixture;
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
    public class DeletePaymentTokenTests
        : IClassFixture<DbContextWebApplicationFactory<Program>>
    {
        private readonly DbContextWebApplicationFactory<Program> _factory;
        private readonly PaymentsDbContext _db;
        private readonly Fixture _fixture;

        private const string MerchantIdHeader = "MerchantId";

        public DeletePaymentTokenTests(DbContextWebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _db = _factory.CreateContext();
            _fixture= new Fixture();
        }

        [Fact]
        public async Task ValidRequest_ShouldReturnNoContent_ShouldDeleteCardDetails()
        {
            // Arrange
            var merchant = _fixture.Create<MerchantPaymentProfile>();
            var cardDetailsToDelete = _fixture.Build<CardDetails>().With(x => x.MerchantId, merchant.MerchantId).Create();

            await _db.Merchants.AddAsync(merchant);
            await _db.CardDetails.AddAsync(cardDetailsToDelete);
            await _db.SaveChangesAsync();
            var client = _factory.CreateClient();
            client.DefaultRequestHeaders.Add(MerchantIdHeader, merchant.MerchantId.ToString());

            // Pre Assert
            _db.CardDetails.Any(
                x => x.CardNumber == cardDetailsToDelete.CardNumber)
                .Should().BeTrue();

            // Act
            var response = await client.DeleteAsync($"/api/paymenttoken/{cardDetailsToDelete.Token}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
            _db.CardDetails.Any(
                x => x.CardNumber == cardDetailsToDelete.CardNumber)
                .Should().BeFalse();
        }

        [Fact]
        public async Task MissingMerchantIdHeader_ShouldReturnBadRequest_ShouldNotDeleteCardDetails()
        {
            // Arrange
            var merchant = _fixture.Create<MerchantPaymentProfile>();
            var cardDetailsToDelete = _fixture.Build<CardDetails>().With(x => x.MerchantId, merchant.MerchantId).Create();

            await _db.Merchants.AddAsync(merchant);
            await _db.CardDetails.AddAsync(cardDetailsToDelete);
            await _db.SaveChangesAsync();
            var client = _factory.CreateClient();

            // Pre Assert
            _db.CardDetails.Any(
                x => x.CardNumber == cardDetailsToDelete.CardNumber)
                .Should().BeTrue();

            // Act
            var response = await client.DeleteAsync($"/api/paymenttoken/{cardDetailsToDelete.Token}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            _db.CardDetails.Any(
                x => x.CardNumber == cardDetailsToDelete.CardNumber)
                .Should().BeTrue();
        }

        [Fact]
        public async Task MerchantDoesntExist_ShouldReturnBadRequest()
        {
            // Arrange
            var merchant = _fixture.Create<MerchantPaymentProfile>();
            var cardDetailsToDelete = _fixture.Build<CardDetails>().With(x => x.MerchantId, merchant.MerchantId).Create();

            await _db.Merchants.AddAsync(merchant);
            await _db.CardDetails.AddAsync(cardDetailsToDelete);
            await _db.SaveChangesAsync();
            var client = _factory.CreateClient();
            client.DefaultRequestHeaders.Add(MerchantIdHeader, "doesntexist");

            // Pre Assert
            _db.CardDetails.Any(
                x => x.CardNumber == cardDetailsToDelete.CardNumber)
                .Should().BeTrue();

            // Act
            var response = await client.DeleteAsync($"/api/paymenttoken/{cardDetailsToDelete.Token}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            _db.CardDetails.Any(
                x => x.CardNumber == cardDetailsToDelete.CardNumber)
                .Should().BeTrue();
        }

        [Fact]
        public async Task WithNoUrlParameter_ShouldReturnMethodNotAllowed()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.DeleteAsync($"/api/paymenttoken/");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.MethodNotAllowed);
        }
    }
}
