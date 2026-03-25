using BarberBoss.Application.UseCases.Billings.GetAll;
using BarberBoss.Application.UseCases.Billings.Register;
using BarberBoss.Communication.Requests;
using BarberBoss.Communication.Responses;
using Microsoft.AspNetCore.Mvc;

namespace BarberBoss.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BillingsController : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(ResponseRegisterBillingJson), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register (
        [FromServices] IRegisterBillingUseCase useCase,
        [FromBody] RequestBillingJson request)
    {
        var response = await useCase.Execute(request);

        return Created(string.Empty, response);
    }

    [HttpGet]
    [ProducesResponseType(typeof(ResponseBillingsJson), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> GetAllBillings([FromServices] IGetAllBillingUseCase useCase)
    {
        var response = await useCase.Execute();

        if (response.Billings.Count != 0)
            return Ok(response);

        return NoContent();

    }
}

