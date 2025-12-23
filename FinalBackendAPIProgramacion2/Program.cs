using Microsoft.EntityFrameworkCore;
using System;
using FinalBackendAPIProgramacion2.Models;
using FinalBackendAPIProgramacion2.Interfaces;
using FinalBackendAPIProgramacion2.Services;

var builder = WebApplication.CreateBuilder(args);

//no anda y no se porque vergas no, puto mcirosoft de mierda, no puede ser mas octuso e impredecible ni a proposito.

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<Final_Programacion_2Context>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<IArticuloService, ArticuloService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
