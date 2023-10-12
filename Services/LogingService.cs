using BankAPI.Data;
using BankAPI.Data.BankModels;
using BankAPI.Data.DTOs;
using Microsoft.EntityFrameworkCore;

namespace BankAPI.Services;

public class LogingService
{
  private readonly BankContext _context;

  public LogingService(BankContext context)
  {
    _context = context;
  }

  public async Task<Administrator?> GetAdmin(AdminDto admin)
  {
    return await _context.Administrators
      .SingleOrDefaultAsync(x => x.Email == admin.Email && x.Pwd == admin.Pwd);
  }
}
