using System.Collections.ObjectModel;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace APP
{
    public class Sensor
    {
        public int Id { get; set; }
        public string Tipo { get; set; }
        public string Grandeza { get; set; }
        public double valor { get; set; }
        public DateTime medido_em { get; set; }
    }

    public class SensorGroup
    {
        public string Tipo { get; set; }
        public List<Sensor> Sensores { get; set; }
    }

    public class MainViewModel : INotifyPropertyChanged
    {
        private static readonly HttpClient client = new HttpClient();

        public ObservableCollection<Sensor> SensoresRecentes { get; set; } = new();
        public ObservableCollection<SensorGroup> SensoresAgrupados { get; set; } = new();

        public MainViewModel()
        {
            _ = CarregarDadosAsync();
        }

        public async Task CarregarDadosAsync()
        {
            try
            {
                // Últimos sensores
                var jsonGrouped = await client.GetStringAsync("http://localhost:5082/api/sensors/grouped");
                var grupos = JsonSerializer.Deserialize<List<SensorGroup>>(jsonGrouped, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                SensoresAgrupados.Clear();
                SensoresRecentes.Clear();

                foreach (var grupo in grupos)
                {
                    // Todos os sensores (para gráficos)
                    SensoresAgrupados.Add(grupo);

                    // Último sensor de cada tipo
                    var ultimo = grupo.Sensores.OrderByDescending(s => s.Id).FirstOrDefault();
                    if (ultimo != null)
                        SensoresRecentes.Add(ultimo);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro ao carregar dados: " + ex.Message);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
