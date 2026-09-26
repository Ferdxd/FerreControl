using FerreControl.Application.Interfaces;
using FerreControl.Infraestructure.Repositories;
using FerreControl.Infraestructure.Services;
using FerreControl.Infrastructure.Persistence;
using FerreControl.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<FerreControlDbContext>(options =>
{
    options.UseSqlite(
        builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddScoped<IEmpleadoRepository, EmpleadoRepository>();
builder.Services.AddScoped<IEmpleadoService, EmpleadoService>();

builder.Services.AddScoped<INegocioRepository, NegocioRepository>();
builder.Services.AddScoped<INegocioService, NegocioService>();
builder.Services.AddScoped<INegocioRepository, NegocioRepository>();
builder.Services.AddScoped<INegocioService, NegocioService>();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();