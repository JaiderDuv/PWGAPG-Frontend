namespace PWGAPG_Frontend.Models;

public class RecetaInsumoDto
{
    public int InsumoId { get; set; }
    public string? InsumoNombre { get; set; }
    public decimal Cantidad { get; set; }
    public string UnidadMedida { get; set; } = "g";
}

public class GuardarRecetaDto
{
    public string Nombre { get; set; } = "";
    public int PorcionesBase { get; set; } = 1;
    public string? Instrucciones { get; set; }
    public List<RecetaInsumoDto> Insumos { get; set; } = new();
}

public class RecetaDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = "";
    public int PorcionesBase { get; set; }
    public string? Instrucciones { get; set; }
    public DateTime FechaCreacion { get; set; }
    public List<RecetaInsumoDto> Insumos { get; set; } = new();
}

// Solo lo necesario para llenar el <select> de insumos.
public class InsumoOpcionDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = "";
    public string UnidadMedida { get; set; } = "";
}

public class ListaInsumosDto
{
    public List<InsumoOpcionDto> Datos { get; set; } = new();
}

public record ApiResultado<T>(bool Exito, T? Datos, string? Mensaje);