using BFFConductor.Api;
using BFFConductor.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddBffResponse(options =>
{
    options.MappingSpecPath = "error-mapping.json";
    options.FallbackDisplayMethod = DisplayMethod.Toast;
});

var app = builder.Build();
app.UseCors(policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
app.UseHttpsRedirection();
app.MapControllers();

app.Run();
