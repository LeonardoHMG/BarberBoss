using BarberBoss.Application.UseCases.Billings.Reports.Excel;
using BarberBoss.Application.UseCases.Billings.Reports.Pdf;
using BarberBoss.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace BarberBoss.Api.Controllers;
[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = Roles.ADMIN)]
public class ReportController : ControllerBase
{
    [HttpGet("excel")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> GetExcel(
        [FromServices] IGenerateBillingsReportExcelUseCase useCase,
        [FromQuery] DateOnly? date)
    {
        byte[] file = await useCase.Execute(date ?? DateOnly.FromDateTime(DateTime.Today));

        if (file.Length > 0)
            return File(file, MediaTypeNames.Application.Octet, "report.xlsx");
        
        return NoContent();
    }

    [HttpGet("pdf")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> GetPdf(
        [FromServices] IGenerateBillingsReportPdfUseCase useCase,
        [FromQuery] DateOnly? date)
    {
        byte[] file  = await useCase.Execute(date ?? DateOnly.FromDateTime(DateTime.Today));

        if (file.Length > 0)
            return File(file, MediaTypeNames.Application.Pdf, "report.pdf"); 

        return NoContent();
    }
}
