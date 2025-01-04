using Business.Utilities.Stripe;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Stripe.Checkout;
using Stripe;
using Business.Services.Abstract;

namespace Presentation.Controllers
{
	public class PaymentController : Controller
	{
		private readonly IPaymentService _paymentService;

		public PaymentController(IPaymentService paymentService)
		{
			_paymentService = paymentService;
		}

		[HttpPost]
		public async Task<IActionResult> Pay()
		{
			var result = await _paymentService.PayAsync();
			switch (result.statusCode)
			{
				case 200:
					return Json(result.id);
				case 400:
					return BadRequest(result.description);
				case 404:
					return NotFound(result.description);
				default:
					return NotFound();
			}
		}

		public async Task<IActionResult> Success(Guid token)
		{
			var IsSucceeded = await _paymentService.PaySuccess(token);
			if (IsSucceeded) return View();
			
			return BadRequest();
		}

		public async Task<IActionResult> Cancel(Guid token)
		{
            var IsSucceeded = await _paymentService.PayCancel(token);
            if (IsSucceeded) return View();

            return BadRequest();
        }
	}
}

