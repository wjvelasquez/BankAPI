using BankAPI.Data.BankModels;
using BankAPI.Data.DTOs;
using BankAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace BankAPI.Controller;

[ApiController]
[Route("[Controller]")]
public class AccountController : ControllerBase
{
  private readonly AccountService _accountService;
  private readonly AccountTypeService _accountTypeService;
  private readonly ClientService _clientService;

  public AccountController(AccountService accountService,
                         AccountTypeService accountTypeService,
                         ClientService clientService)
  {
    _accountService = accountService;
    _accountTypeService = accountTypeService;
    _clientService = clientService;
  }

  [HttpGet]
  public async Task<IEnumerable<AccountDtoOut>> Get()
  {
    return await _accountService.GetAll();
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<AccountDtoOut>> GetById(int id)
  {
    var account = await _accountService.GetDtoById(id);

    if (account is null)
      return AccountNotFound(id);

    return Ok(account);
  }

  [HttpPost]
  public async Task<ActionResult<Client>> Create(AccountDtoIn account)
  {
    var validationResult = await ValidateAccount(account);

    if (!validationResult.Equals("valid"))
      return BadRequest(new { message = validationResult });

    var newAccount = await _accountService.Create(account);

    return CreatedAtAction(nameof(GetById), new { id = newAccount.Id }, newAccount);
  }

  [HttpPut("{id}")]
  public async Task<IActionResult> Update(int id, AccountDtoIn account)
  {
    if (id != account.Id)
      return BadRequest(new { message = $"El Id = {id} en la URL no conincide con el Id {account.Id} en el objeto enviado" });

    var accountToUpdate = await _accountService.GetById(id);

    if (accountToUpdate is not null)
    {
      string validationResult = await ValidateAccount(account);

      if (!validationResult.Equals("valid"))
        return BadRequest(new { message = validationResult });

      await _accountService.Update(account);
      return NoContent();
    }
    else
      return AccountNotFound(id);
  }

  [HttpDelete("{id}")]
  public async Task<IActionResult> Delete(int id)
  {
    var accountToDelete = await _accountService.GetById(id);

    if (accountToDelete is not null)
    {
      await _accountService.Delete(id);
      return Ok();
    }
    else
      return AccountNotFound(id);
  }

  [NonAction]
  public NotFoundObjectResult AccountNotFound(int id)
  {
    return NotFound(new { message = $"Cuenta con Id = {id} No existe" });
  }

  [NonAction]
  public async Task<string> ValidateAccount(AccountDtoIn account)
  {
    string result = "valid";

    var accountType = await _accountTypeService.GetById(account.AccountType);

    if (accountType is null)
      result = $"El tipo de cuenta {account.AccountType} no existe";

    var clientId = account.ClientId.GetValueOrDefault();
    var client = await _clientService.GetById(clientId);

    if (client is null)
      result = $"El cliente {clientId} no existe";

    return result;
  }
}
