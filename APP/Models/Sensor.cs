namespace APP.Models
{
    public class Sensor
    {
        public int? Id { get; set; }
        public string? Tipo { get; set; }
        public string? Grandeza { get; set; }
        public double? valor { get; set; }
        public DateTime? medido_em { get; set; }
        public bool? deleted { get; set; } = false;
    }
}
