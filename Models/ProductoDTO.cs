using System.ComponentModel.DataAnnotations;

namespace PWGAPG_Frontend.Models
{
    public class ProductoDTO
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string? ImagenBase64 { get; set; }

        [Required(ErrorMessage = "El nombre del producto es obligatorio.")]
        public string Nombre { get; set; } = string.Empty;

        public List<string> Ingredientes { get; set; } = new();

        [Required(ErrorMessage = "La receta es obligatoria.")]
        public string Receta { get; set; } = string.Empty;

        [Required(ErrorMessage = "La cantidad es obligatoria.")]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0.")]
        public int Cantidad { get; set; } = 1;

        [Required(ErrorMessage = "El precio es obligatorio.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor a 0.")]
        public decimal Precio { get; set; }
    }
}