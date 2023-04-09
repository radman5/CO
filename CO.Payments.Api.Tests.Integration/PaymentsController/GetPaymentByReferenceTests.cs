using AutoFixture;
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

    public GetPaymentByReferenceTests(DbContextWebApplicationFactory<Program> factory, Fixture fixture)
    {
        _factory = factory;
        _db = _factory.CreateContext();
        _fixture = fixture;
    }

    [Fact]
    public async Task WhenPaymentExists_ReturnPayment()
    {
        // Arrange
        var payment = _fixture.Create<Payment>();
        await _db.Payments.AddAsync(payment);
        await _db.SaveChangesAsync();
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add(MerchantIdHeader, payment.MerchantId.ToString());

        // Act
        var response = await client.GetAsync($"/api/payments/{payment.PaymentReference}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var paymentResponse = await response.Content.ReadFromJsonAsync<GetPaymentResponse>();
        paymentResponse.PaymentReference.Should().Be(payment.PaymentReference);
    }
}
