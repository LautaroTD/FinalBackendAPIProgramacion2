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
builder.Services.AddScoped<IResenaService, ResenaService>();
builder.Services.AddScoped<IArticuloRelacionadoService, ArticuloRelacionadoService>();
builder.Services.AddTransient<IImagenService, ImagenService>();


builder.Services.AddCors(options =>
{
    options.AddPolicy("BlazorWasm", policy =>
    {
        policy
        //WithOrigins es el puerto cliente permitido, NO el puerto de la api, sino del CLIENTE.
            .WithOrigins("https://localhost:7278") // ACA EL PUERTO DEL CLIENTE //podes averiguar cual es ejecutando el cliente y viendo que te pone el visual
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("BlazorWasm");

app.UseStaticFiles(); //necesario para poder usar wwwroot y acceder a las imagenes de FORMA ESTATICA Y PUBLICA

app.UseAuthorization();

app.MapControllers();

app.Run();
