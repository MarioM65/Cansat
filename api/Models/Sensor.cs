using System;
using System.ComponentModel.DataAnnotations;

namespace Cansat.Models
{
    public class Sensor
    {
        public int? Id { get; set; }

        [Required(ErrorMessage = "O campo Tipo é obrigatório.")]
        public string? Tipo { get; set; }

        [Required(ErrorMessage = "O campo Grandeza é obrigatório.")]
        public string? Grandeza { get; set; }

        [Required(ErrorMessage = "O campo valor é obrigatório.")]
        public double? valor { get; set; }

        [Required(ErrorMessage = "O campo medido_em é obrigatório.")]
        public DateTime? medido_em { get; set; }

        public bool? deleted { get; set; } = false;
    }
}
