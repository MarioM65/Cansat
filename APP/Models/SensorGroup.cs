using System.Collections.Generic;

namespace APP.Models
{
    public class SensorGroup
    {
        public string? Tipo { get; set; }
        public List<Sensor>? Sensores { get; set; }
    }
    public class SensorUltimoGrupo
{
    public string Tipo { get; set; }
    public Sensor UltimoSensor { get; set; }
}

}
