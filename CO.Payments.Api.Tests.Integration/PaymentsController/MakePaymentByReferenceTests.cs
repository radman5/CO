using AutoFixture;
using AutoFixture.AutoMoq;
using Castle.Core.Internal;
using CO.Payments.Api.Data.Database;
using CO.Payments.Api.Data.DbModels;
using CO.Payments.Api.Data.DTOs;
using FluentAssertions;
using NuGet.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace CO.Payments.Api.Tests.Integration.PaymentsController;

public class MakePaymentTests
    : IClassFixture<DbContextWebApplicationFactory<Program>>
{
    private readonly DbContextWebApplicationFactory<Program> _factory;
    private readonly PaymentsDbContext _db;
    private readonly Fixture _fixture;

    private const string MerchantIdHeader = "MerchantId";

    public MakePaymentTests(DbContextWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _db = _factory.CreateContext();
        _fixture = new Fixture();
        _fixture.Customize(new AutoMoqCustomization { ConfigureMembers = true });
    }

    [Fact]
    public async Task WithValidRequest_ShouldReturnCreated_ShouldCreateSuccessfulPayment()
    {
        var newMerchant = await CreateAndSaveMerchant();
        var newCardDetails = await CreateAndSaveCardDetails(newMerchant.MerchantId);
        var makePaymentRequest = new MakePaymentRequest
        {
            Token = newCardDetails.Token,
            Amount = 100,
            Currency = "GBP",
        };
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add(MerchantIdHeader, newMerchant.MerchantId.ToString());

        // Pre Assert
        _db.Payments.Any(
            x => x.MerchantId == newMerchant.MerchantId)
            .Should().BeFalse();

        // Act
        var response = await client.PostAsJsonAsync($"/api/payments/", makePaymentRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        _db.Payments.Any(
            x => x.MerchantId == newMerchant.MerchantId &&
            x.PaymentStatus == Data.Lookups.PaymentStatus.Success)
            .Should().BeTrue();
    }

    [Fact]
    public async Task CardWithInsufficientFunds_ShouldReturnUnprocessable_ShouldCreateFailedPayment()
    {
        var newMerchant = await CreateAndSaveMerchant();
        var newCardDetails = await CreateAndSaveCardDetails(newMerchant.MerchantId, "insuffientfunds");
        var makePaymentRequest = new MakePaymentRequest
        {
            Token = newCardDetails.Token,
            Amount = 100,
            Currency = "GBP",
        };
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add(MerchantIdHeader, newMerchant.MerchantId.ToString());

        // Pre Assert
        _db.Payments.Any(
            x => x.MerchantId == newMerchant.MerchantId)
            .Should().BeFalse();

        // Act
        var response = await client.PostAsJsonAsync($"/api/payments/", makePaymentRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        _db.Payments.Any(
            x => x.MerchantId == newMerchant.MerchantId &&
            x.PaymentStatus == Data.Lookups.PaymentStatus.Failed)
            .Should().BeTrue();
    }

    [Fact]
    public async Task WhenCardDetailsAlreadyUsed_ShouldReturnBadRequest_ShouldNotCreatePayment()
    {
        var newMerchant = await CreateAndSaveMerchant();
        var newCardDetails = await CreateAndSaveCardDetails(newMerchant.MerchantId, "insuffientfunds");
        newCardDetails.Use();
        await _db.SaveChangesAsync();
        var makePaymentRequest = new MakePaymentRequest
        {
            Token = newCardDetails.Token,
            Amount = 100,
            Currency = "GBP",
        };
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add(MerchantIdHeader, newMerchant.MerchantId.ToString());

        // Pre Assert
        _db.Payments.Any(
            x => x.MerchantId == newMerchant.MerchantId)
            .Should().BeFalse();

        // Act
        var response = await client.PostAsJsonAsync($"/api/payments/", makePaymentRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        _db.Payments.Any(
            x => x.MerchantId == newMerchant.MerchantId)
            .Should().BeFalse();
    }

    private async Task<Payment> GeneratePayment()
    {
        var merchant = await CreateAndSaveMerchant();
        var cardDetails = await CreateAndSaveCardDetails(merchant.MerchantId);

        var makePaymentRequest = _fixture.Build<MakePaymentRequest>()
            .With(x => x.Token, cardDetails.Token).Create();

        return await CreateAndSavePayment(merchant, cardDetails, makePaymentRequest);
    }

    private async Task<CardDetails> CreateAndSaveCardDetails(long merchantId, string cardNumber = "")
    {
        cardNumber = cardNumber.IsNullOrEmpty() ? _fixture.Create<string>() : cardNumber;
        var newCardDetails = CardDetails.Create(
            cardNumber,
            _fixture.Create<string>(),
            _fixture.Create<string>(),
            _fixture.Create<string>(),
            merchantId);
        await _db.CardDetails.AddAsync(newCardDetails);
        await _db.SaveChangesAsync();

        return newCardDetails;
    }

    private async Task<MerchantPaymentProfile> CreateAndSaveMerchant()
    {
        var newMerchant = MerchantPaymentProfile.Create("123abc");
        await _db.Merchants.AddAsync(newMerchant);
        await _db.SaveChangesAsync();

        return newMerchant;
    }

    private async Task<Payment> CreateAndSavePayment(MerchantPaymentProfile merchant, CardDetails cardDetails, MakePaymentRequest makePaymentRequest)
    {
        var newPayment = Payment.Create(
            makePaymentRequest,
            merchant.MerchantId,
            cardDetails);

        await _db.Payments.AddAsync(newPayment);
        await _db.SaveChangesAsync();
        return newPayment;
    }
}
