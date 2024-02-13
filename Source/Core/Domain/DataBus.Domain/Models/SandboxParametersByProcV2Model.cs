namespace DataBus.Domain;

public record SandboxParametersByProcV2Model
{
    public bool Succeeded { get; set; }
    public bool ValidateParams { get; set; }
    public IList<SandboxParametersByProcModel>? Procedures { get; set; }
}
