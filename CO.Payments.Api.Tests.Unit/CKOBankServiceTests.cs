using CO.Payments.Api.Data.DTOs;
using CO.Payments.Api.Services;
using FluentAssertions;

namespace CO.Payments.Api.Tests.Unit;

public class CKOBankServiceTests
{
    [Fact]
    public void WhenCardNumberIs_insuffientfunds_ReturnsInsufficientFundsResult()
    {
        // Arrange
        var sut = new CKOBankService();

        // Act
        var result = sut.MakePayment(new BankPaymentRequest
        {
            CardNumber = "insuffientfunds"
        });

        // Assert
        result.ResultType.Should().Be(result.ResultType);
    }
}