namespace BFFConductor.Configuration;

public class BffResponseOptions
{
    public string MappingSpecPath { get; set; } = "error-mapping.json";
    public string FallbackDisplayMethod { get; set; } = "toast";
}
