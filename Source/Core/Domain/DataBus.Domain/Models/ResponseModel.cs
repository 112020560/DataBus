using System.Text;

namespace DataBus.Domain;

public record ResponseModel
{
    public object? ResponseData { get; set; }
    public StringBuilder? StrLog { get; set; }
    //public BackEndRequest CommonObject { get; set; }
    //public BackEndResponse ResponseObject { get; set; }
    public string? Ejecucion { get; set; }
    public string? Servidor { get; set; }
    public string? BaseDatos { get; set; }
    public int EnableTrace { get; set; }
}
