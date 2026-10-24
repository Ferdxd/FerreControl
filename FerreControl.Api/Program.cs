using FerreControl.Application.Interfaces.IRepository;
using FerreControl.Application.Interfaces.IService;
using FerreControl.Infraestructure.Repositories;
using FerreControl.Infraestructure.Services;
using FerreControl.Infrastructure.Persistence;
using FerreControl.Infrastructure.Repositories;
using FerreControl.Infrastructure.Services;
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

builder.Services.AddScoped<ISalarioEmpleadoRepository, SalarioEmpleadoRepository>();
builder.Services.AddScoped<ISalarioEmpleadoService, SalarioEmpleadoService>();

builder.Services.AddScoped<IHorarioRepository, HorarioRepository>();
builder.Services.AddScoped<IHorarioService, HorarioService>();

builder.Services.AddScoped<IHorarioDetalleRepository, HorarioDetalleRepository>();
builder.Services.AddScoped<IHorarioDetalleService, HorarioDetalleService>();

builder.Services.AddScoped<IEmpleadoHorarioRepository, EmpleadoHorarioRepository>();
builder.Services.AddScoped<IEmpleadoHorarioService, EmpleadoHorarioService>();

builder.Services.AddScoped<IEmpleadoCredencialRepository, EmpleadoCredencialRepository>();
builder.Services.AddScoped<IEmpleadoCredencialService, EmpleadoCredencialService>();

builder.Services.AddScoped<IMarcacionAsistenciaRepository, MarcacionAsistenciaRepository>();
builder.Services.AddScoped<IMarcacionAsistenciaService, MarcacionAsistenciaService>();

builder.Services.AddScoped<INominaRepository, NominaRepository>();
builder.Services.AddScoped<INominaService, NominaService>();

builder.Services.AddScoped<INominaDetalleRepository, NominaDetalleRepository>();
builder.Services.AddScoped<INominaDetalleService, NominaDetalleService>();

builder.Services.AddScoped<IPagoNominaRepository, PagoNominaRepository>();
builder.Services.AddScoped<IPagoNominaService, PagoNominaService>();

builder.Services.AddScoped<IAdelantoEmpleadoRepository, AdelantoEmpleadoRepository>();
builder.Services.AddScoped<IAdelantoEmpleadoService, AdelantoEmpleadoService>();

builder.Services.AddScoped<IAdelantoAplicacionRepository, AdelantoAplicacionRepository>();
builder.Services.AddScoped<IAdelantoAplicacionService, AdelantoAplicacionService>();

builder.Services.AddScoped<IReglaHoraExtraRepository, ReglaHoraExtraRepository>();
builder.Services.AddScoped<IEmpleadoReglaHoraExtraRepository, EmpleadoReglaHoraExtraRepository>();
builder.Services.AddScoped<IRegistroHoraExtraRepository, RegistroHoraExtraRepository>();
builder.Services.AddScoped<IHoraExtraService, HoraExtraService>();

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
