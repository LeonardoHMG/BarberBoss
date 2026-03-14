using BarberBoss.Application.UseCases.Billings.Register;
using BarberBoss.Communication.Requests;
using Microsoft.AspNetCore.Mvc;

namespace BarberBoss.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BillingsController : ControllerBase
{
    [HttpPost]
    public IActionResult Create([FromBody] RequestBillingJson request)
    {
        var useCase = new RegisterBillingUseCase();

        var response = useCase.Execute(request);

        return Created(string.Empty, response);
    }
}

