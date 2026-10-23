namespace PWGAPG_Frontend.Models;

public class UmbralMargenDto
{
    public decimal UmbralMargenMinimo { get; set; }
}

public class ProductoEvaluacionDto
{
    public int ProductoId { get; set; }
    public string Nombre { get; set; } = "";

    // Costo directo (recetas + empaques): base del margen principal.
    public decimal CostoDirecto { get; set; }

    // Costo completo con fijos, solo como referencia de análisis.
    public decimal CostoTotal { get; set; }

    public decimal? PrecioVenta { get; set; }

    // Margen directo: el que se evalúa por defecto en el menú.
    public decimal? MargenDirecto { get; set; }

    // Margen con costo completo, solo como referencia.
    public decimal? MargenReal { get; set; }

    public bool MargenNoCalculable { get; set; }
    public string? MotivoNoCalculable { get; set; }
    public bool EnRiesgo { get; set; }
}

public class EvaluacionMenuDto
{
    public decimal UmbralMargenMinimo { get; set; }
    public List<ProductoEvaluacionDto> Productos { get; set; } = new();
}