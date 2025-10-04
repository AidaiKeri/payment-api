using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaymentApi.DTOs;
using PaymentApi.Services.Interfaces;
using System.Security.Claims;

namespace PaymentApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentController : ControllerBase
{
    private readonly IPaymentService _paymentService;

    public PaymentController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    [Authorize]
    [HttpPost("pay")]
    public async Task<ActionResult<PaymentResult>> Pay()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdClaim))
            return Unauthorized();

        var userId = Guid.Parse(userIdClaim);

        var result = await _paymentService.PayAsync(userId);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }
}
