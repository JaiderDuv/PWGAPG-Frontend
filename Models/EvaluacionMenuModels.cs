namespace PWGAPG_Frontend.Models;

public class UmbralMargenDto
{
    public decimal UmbralMargenMinimo { get; set; }
}

public class ProductoEvaluacionDto
{
    public int ProductoId { get; set; }
    public string Nombre { get; set; } = "";
    public decimal CostoTotal { get; set; }
    public decimal? PrecioVenta { get; set; }
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