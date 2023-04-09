using AutoFixture;
using AutoFixture.AutoMoq;
using CO.Payments.Api.Controllers.ExceptionHandling;
using CO.Payments.Api.Data.DTOs;
using CO.Payments.Api.Services;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CO.Payments.Api.Tests.Unit
{
    public class PaymentServiceTests
    {
        private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization { ConfigureMembers = true });

        [Fact]
        public void WhenMerchantNotFound_ReturnThrowHttpResponseException()
        {
            var sut = _fixture.Build<PaymentService>().Create();
            var makePaymentRequest = _fixture.Create<MakePaymentRequest>();
            var merchantId = _fixture.Create<long>();

            // Act
            var action = () => sut.MakePaymentToAquiringBank(makePaymentRequest, merchantId);

            // Arrange
            action.Should().ThrowAsync<HttpResponseException>();
        }
    }
}
