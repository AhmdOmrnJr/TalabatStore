using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using System.Reflection.Metadata;
using Talabat.APIs.Errors;
using Talabat.Core.Entities;
using Talabat.Core.Entities.OrderAggregate;
using Talabat.Core.Services.Contract;

namespace Talabat.APIs.Controllers
{
    public class PaymentsController : BaseController
    {
        private readonly IPaymentService _paymentService;
        private readonly ILogger<PaymentsController> _logger;
        private const string endpointSecret = " whsec_a8cac15484c6fd6cbe10d0ceb5b9070a6285bd435c82c88d24ddfd23e95975ac";

        public PaymentsController(IPaymentService paymentService, ILogger<PaymentsController> logger)
        {
            _paymentService = paymentService;
            _logger = logger;
        }

        [Authorize]
        [ProducesResponseType(typeof(CustomerBasket), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [HttpPost("{basketId}")]
        public async Task<ActionResult<CustomerBasket>> CreateOrUpdatePaymentIntent(string basketId)
        {
            var basket = await _paymentService.CreateOrUpdatePaymentIntent(basketId);

            if (basket is null) return BadRequest(new ApiResponse(400, "There Is An Error With Your Basket"));

            return Ok(basket);
        }

        [HttpPost("webhook")]
        public async Task<IActionResult> WebHook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

            var stripeEvent = EventUtility.ParseEvent(json);

            var signatureHeader = Request.Headers["Stripe-Signature"];

            stripeEvent = EventUtility.ConstructEvent(json, signatureHeader, endpointSecret);

            var paymentIntent = (PaymentIntent) stripeEvent.Data.Object;

            Order order;

            switch (stripeEvent.Type)
            {
                case Events.PaymentIntentSucceeded:
                    order = await _paymentService.UpdatePaymentIntentToSucceeddedOrFailed(paymentIntent.Id, true);
                    _logger.LogInformation("Payment Successful", paymentIntent.Id);
                    break;

                case Events.PaymentIntentPaymentFailed:
                    order = await _paymentService.UpdatePaymentIntentToSucceeddedOrFailed(paymentIntent.Id, false);
                    _logger.LogInformation("Payment Failed", paymentIntent.Id);
                    break;
            }

            return Ok();
        }
    }
}
