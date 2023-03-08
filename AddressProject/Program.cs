using Microsoft.EntityFrameworkCore;
using AddressProject.DB;
using NuGet.Protocol;
using AddressProject.Configurations;
//using AddressProject.Entities;
var builder = WebApplication.CreateBuilder(args);


// Add services to the container.

builder.Services.AddControllers();
//Register database context
builder.Services.AddDbContext<AddressDataContex>();
//opt.UseInMemoryDatabase("AddressDB"));// //
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors();
//Inject AutoMapper Dependency
builder.Services.AddAutoMapper(typeof(MapperConfig));
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
