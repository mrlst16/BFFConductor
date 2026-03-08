using BFFConductor.Api;
using BFFConductor.Api.Services;
using BFFConductor.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddBffResponse(options =>
{
    options.MappingSpecPath = "error-mapping.json";
    options.FallbackDisplayMethod = DisplayMethod.Toast;
});

builder.Services.AddScoped<IWeatherService, WeatherService>();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
