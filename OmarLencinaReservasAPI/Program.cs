using System;
using Microsoft.EntityFrameworkCore;
using OmarLencinaReservasAPI.Context;

var builder = WebApplication.CreateBuilder(args);

// :D Variables de conexión
var connectionString = builder.Configuration.GetConnectionString("Connection");

// :D Registro de servicios
builder.Services.AddDbContext<ReservasContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddOpenApi();

// :D Habilita el CORS para permitir solicitudes desde cualquier origen
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// :D Aplica CORS antes de Authorization
app.UseCors("AllowAll");

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", ":D Ol - API Reservas 2025"));

app.Run();
