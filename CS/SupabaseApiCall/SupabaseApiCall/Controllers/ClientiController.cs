using Microsoft.AspNetCore.Mvc;
using SupabaseApiCall.Contracts.Anagrafiche;
using SupabaseApiCall.Contracts.DatiFinali;
using SupabaseApiCall.Contracts.DatiIntermedi;
using SupabaseApiCall.Services;
using System;

namespace SupabaseApiCall.Controllers;

[ApiController]
[Route("clienti")]
public class ClientiController : ControllerBase
{
    private readonly IClienteService _service;
    private readonly IFattureService _fattureService;

    public ClientiController(IClienteService service, IFattureService fattureService)
    {
        _service = service;
        _fattureService = fattureService;
    }

    [HttpGet("")]
    public async Task<ActionResult<IReadOnlyList<ClienteResponse>>> Get()
    {
        var result = await _service.GetClientiAsync();
        return Ok(result);
    }
    //Per test clienti/17790
    [HttpGet("{codiceCliente:long}")]
    public async Task<ActionResult<ClienteResponse>> GetById(long codiceCliente)
    {
        var cliente = await _service.GetClienteAsync(codiceCliente);
        
        if (cliente is null)
            return NotFound();

        return Ok(cliente);
    }
    //Per test /clienti/17790/fatturato?dal=2024-01-01&al=2025-12-31
    [HttpGet("{codiceCliente:long}/fatturato")]
    public async Task<ActionResult<List<FatturatoMensileResponse>>> GetFatturatoMensile(
        long codiceCliente,
        [FromQuery] DateTime dal,
        [FromQuery] DateTime al)
    { 
        if (al < dal)
            return BadRequest("Intervallo date non valido (al < dal).");

        var result = await _fattureService
            .GetFatturatoMensileAsync(codiceCliente, dal, al);

        return Ok(result);
    }
    //Per test /clienti/17790/fatture?dal=2024-01-01&al=2025-12-31
    [HttpGet("{codiceCliente:long}/fatture")]
    public async Task<ActionResult<List<FatturaClienteResponse>>> GetFatture(
        long codiceCliente,
        [FromQuery] DateTime dal,
        [FromQuery] DateTime al)
    {
        if (al < dal)
            return BadRequest("Intervallo date non valido (al < dal).");

        var result = await _fattureService
            .GetListaFattureAsync(codiceCliente, dal, al);

        return Ok(result);
    }
    [HttpGet("{codiceCliente:long}/statistiche")]
    public async Task<ActionResult<List<StatMensiliMedieResponse>>> GetStatistiche(
        long codiceCliente,
        [FromQuery] DateTime dal,
        [FromQuery] DateTime al,
        [FromQuery] int k = 3)
    {
        if (al < dal)
            return BadRequest("Intervallo date non valido (al < dal).");
        if (k <= 0)
            return BadRequest("k deve essere maggiore di 0.");

        if (k > 24)
            return BadRequest("k troppo grande (max 24 mesi).");
        var result = await _fattureService
            .GetStatisticheFattureAsync(codiceCliente, dal, al , k);
        return Ok(result);
    }


}
