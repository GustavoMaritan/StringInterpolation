using ApiTests;
using StringInterpolation.Core;
using StringInterpolation.Core.Domain;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;

services.AddControllers();
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

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseInterpolationReload()
    .UseInterpolation();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
