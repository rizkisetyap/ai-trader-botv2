using backend_net.Objects.Dtos;
using backend_net.Services;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/v1/[controller]")]
public class SahamController : ControllerBase
{
    private readonly AnalisaService _analisaService;

    public SahamController(AnalisaService analisaService)
    {
        _analisaService = analisaService;
    }

    [HttpPost("terima-analisa")]
    public async Task<IActionResult> RequestAnalisa([FromBody] AnalisaRequestDto request)
    {
        await _analisaService.AddAnalisaRequestAsync(request);
        return Accepted(new { message = "Permintaan diterima, sedang diproses AI." });
    }

    [HttpGet("get-analisa-result")]
    public async Task<IActionResult> GetAnalisaResult()
    {
        var result = await _analisaService.GetAllAnalisaRequestsAsync();
        return Ok(result);
    }

    [HttpGet("get-analisa-result/{id:guid}")]
    public async Task<IActionResult> GetAnalisaResultById(Guid id)
    {
        var result = await _analisaService.GetAnalisaRequestByIdAsync(id);
        return result == null ? NotFound() : Ok(result);
    }

    [HttpDelete("get-analisa-result/{id:guid}")]
    public async Task<IActionResult> DeleteAnalisaResult(Guid id)
    {
        var deleted = await _analisaService.DeleteAnalisaRequestAsync(id);
        return deleted ? NoContent() : NotFound();
    }
}