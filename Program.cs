using System.Text;
using BankAPI.Data;
using BankAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//inyectamos la conexion
builder.Services.AddSqlServer<BankContext>(builder.Configuration.GetConnectionString("BankConnection"));

//inyectamos servicios
builder.Services.AddScoped<ClientService>();
builder.Services.AddScoped<AccountService>();
builder.Services.AddScoped<AccountTypeService>();
builder.Services.AddScoped<LogingService>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
  .AddJwtBearer(options =>
  {
    options.TokenValidationParameters = new TokenValidationParameters
    {
      ValidateIssuerSigningKey = true,
      IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"]!)),
      ValidateIssuer = false,
      ValidateAudience = false
    };
  });

builder.Services.AddAuthorization(options =>
{
  options.AddPolicy("SuperAdmin", policy => policy.RequireClaim("AdminType", "Super"));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
