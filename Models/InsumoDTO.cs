using System.ComponentModel.DataAnnotations;

namespace PWGAPG_Frontend.Models
{
    public class InsumoDTO
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string? ImagenBase64 { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        public string Nombre { get; set; } = string.Empty;

        public string? Marca { get; set; }

        [Required(ErrorMessage = "La cantidad es obligatoria.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0.")]
        public decimal Cantidad { get; set; }

        [Required(ErrorMessage = "Seleccione una unidad de medida.")]
        public string UnidadMedida { get; set; } = string.Empty;

        [Required(ErrorMessage = "El precio es obligatorio.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor a 0.")]
        public decimal Precio { get; set; }
    }
}