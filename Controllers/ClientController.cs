using BankAPI.Data;
using BankAPI.Data.BankModels;
using BankAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankAPI.Controller;

[Authorize]
[ApiController]
[Route("api/[Controller]")]

public class ClientController : ControllerBase
{
  private readonly ClientService _service;

  public ClientController(ClientService service)
  {
    _service = service;
  }

  [HttpGet("getall")]
  public async Task<IEnumerable<Client>> Get()
  {
    return await _service.GetAll();
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<Client>> GetById(int id)
  {
    var client = await _service.GetById(id);

    if (client is null)
      return ClientNotFound(id);

    return client;
  }

  [HttpPost("create")]
  public async Task<IActionResult> Create(Client client)
  {
    var newClient = await _service.Create(client);

    return CreatedAtAction(nameof(GetById), new { id = newClient.Id }, newClient);
  }

  [HttpPut("update/{id}")]
  public async Task<IActionResult> Update(int id, Client client)
  {
    if (id != client.Id)
      return BadRequest(new { message = $"El ID ({id}) no coincide con del ID ({client.Id} del cuerpo de la solicitud)" });

    var clientToUpdate = await _service.GetById(id);

    if (clientToUpdate is not null)
    {
      await _service.Update(id, client);
      return NoContent();
    }
    else
      return ClientNotFound(id);
  }

  [HttpDelete("delete/{id}")]
  public async Task<IActionResult> Delete(int id)
  {
    var clientToDelete = await _service.GetById(id);

    if (clientToDelete is not null)
    {
      await _service.Delete(id);
      return Ok();
    }
    else
      return ClientNotFound(id);
  }

  [NonAction]
  public NotFoundObjectResult ClientNotFound(int id)
  {
    return NotFound(new { message = $"El cliente con ID {id} no existe" });
  }
}