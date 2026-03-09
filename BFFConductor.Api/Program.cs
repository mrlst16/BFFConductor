using BFFConductor.Api;
using BFFConductor.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy => policy
        .AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader()
        .WithExposedHeaders("x-handle-message-as"));
});

builder.Services.AddBffResponse(options =>
{
    options.MappingSpecPath = "error-mapping.json";
    options.FallbackDisplayMode = DisplayMethod.Toast;
});

var app = builder.Build();

app.UseCors();
app.UseHttpsRedirection();
app.MapControllers();

app.Run();
