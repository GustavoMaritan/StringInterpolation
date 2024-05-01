using ApiTests;
using StringInterpolation.Core;
using StringInterpolation.Core.Domain;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var services = builder.Services;

services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();


services
    .AddInterpolation(x =>
    {
        x.SetProvider<MeuProvider>();
        x.SetSearchKey(SearchKey.Query, "nome");
    })
    .AddInterpolationService();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseInterpolation();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
