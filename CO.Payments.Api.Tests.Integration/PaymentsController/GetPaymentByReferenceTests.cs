using AutoFixture;
using AutoFixture.AutoMoq;
using CO.Payments.Api.Data.Database;
using CO.Payments.Api.Data.DbModels;
using CO.Payments.Api.Data.DTOs;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace CO.Payments.Api.Tests.Integration.PaymentsController;

public class GetPaymentByReferenceTests
    : IClassFixture<DbContextWebApplicationFactory<Program>>
{
    private readonly DbContextWebApplicationFactory<Program> _factory;
    private readonly PaymentsDbContext _db;
    private readonly Fixture _fixture;

    private const string MerchantIdHeader = "MerchantId";

    public GetPaymentByReferenceTests(DbContextWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _db = _factory.CreateContext();
        _fixture = new Fixture();
        _fixture.Customize(new AutoMoqCustomization { ConfigureMembers = true });
    }

    [Fact]
    public async Task WhenPaymentExists_ReturnPayment()
    {
        // Arrange
        var payment = await GeneratePayment();
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add(MerchantIdHeader, payment.MerchantId.ToString());

        // Act
        var response = await client.GetAsync($"/api/payments/{payment.PaymentReference}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var paymentResponse = await response.Content.ReadFromJsonAsync<GetPaymentResponse>();
        paymentResponse.PaymentReference.Should().Be(payment.PaymentReference);
    }

    [Fact]
    public async Task WhenPaymentDoesNotExist_ReturnNotFound()
    {
        // Arrange
        var payment = await GeneratePayment();
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add(MerchantIdHeader, payment.MerchantId.ToString());

        // Act
        var response = await client.GetAsync($"/api/payments/doesntexist");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task WhenUrlQueryEmpty_ReturnBadRequest()
    {
        // Arrange
        var payment = await GeneratePayment();
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add(MerchantIdHeader, payment.MerchantId.ToString());

        // Act
        var response = await client.GetAsync($"/api/payments/");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task WhenMerchantIdHeaderMissing_ReturnBadRequest()
    {
        // Arrange
        var payment = await GeneratePayment();
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync($"/api/payments/doesntexist");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    private async Task<Payment> GeneratePayment()
    {
        var merchant = MerchantPaymentProfile.Create("123abc");
        await _db.Merchants.AddAsync(merchant);
        await _db.SaveChangesAsync();

        var cardDetails = CardDetails.Create(
            _fixture.Create<string>(),
            _fixture.Create<string>(),
            _fixture.Create<string>(),
            _fixture.Create<string>(),
            merchant.MerchantId);
        await _db.CardDetails.AddAsync(cardDetails);
        var makePaymentRequest = _fixture.Build<MakePaymentRequest>()
            .With(x => x.Token, cardDetails.Token).Create();

        var payment = Payment.Create(
            makePaymentRequest,
            merchant.MerchantId,
            cardDetails);

        await _db.Payments.AddAsync(payment);
        await _db.SaveChangesAsync();
        return payment;
    }
}
