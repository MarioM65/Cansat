using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using APP.Models;
using APP.Views;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using LiveChartsCore.Measure;
using LiveChartsCore.Defaults;

namespace APP.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private static readonly HttpClient client = new();

        public ObservableCollection<Sensor> SensoresRecentes { get; set; } = new();
        public ObservableCollection<SensorGroup> SensoresAgrupados { get; set; } = new();
public ObservableCollection<SensorGrafico> Graficos { get; set; } = new();

        private UserControl _paginaAtual;
        public UserControl PaginaAtual
        {
            get => _paginaAtual;
            set
            {
                _paginaAtual = value;
                OnPropertyChanged();
            }
        }

        public ICommand IrParaInicioCommand { get; }
        public ICommand IrParaGraficosCommand { get; }

        public MainViewModel()
        {
            IrParaInicioCommand = new RelayCommand(_ => IrParaInicio());
            IrParaGraficosCommand = new RelayCommand(_ => IrParaGraficos());

            _ = CarregarDadosAsync();
        }

        private void IrParaInicio() => PaginaAtual = new InicioView { DataContext = this };
        private void IrParaGraficos() => PaginaAtual = new GraficosView { DataContext = this };

        public async Task CarregarDadosAsync()
        {
            await CarregarGruposAsync();
            await CarregarUltimosSensoresAsync();
            IrParaInicio();
        }

      public async Task CarregarGruposAsync()
{
    try
    {
        var jsonGrouped = await client.GetStringAsync("http://localhost:5082/api/sensors/grouped");
        var grupos = JsonSerializer.Deserialize<List<SensorGroup>>(jsonGrouped, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        SensoresAgrupados.Clear();
        Graficos.Clear();

        foreach (var grupo in grupos)
        {
            SensoresAgrupados.Add(grupo);

            var pontos = grupo.Sensores?
                .OrderBy(s => s.medido_em)
                .Where(s => s.valor.HasValue && s.medido_em.HasValue)
                .Select(s => new ObservablePoint(s.medido_em.Value.Ticks, s.valor.Value))
                .ToList();

            if (pontos != null && pontos.Any())
            {Console.WriteLine(pontos);
                Graficos.Add(new SensorGrafico
                {
                    Tipo = grupo.Tipo,
                    Series = new ISeries[]
                    {
                        new LineSeries<ObservablePoint>
                        {
                            Values = pontos,
                            GeometrySize = 8,
                            Stroke = new SolidColorPaint(SKColors.Blue, 2),
                            Fill = null
                        }
                    },
                XAxes = new Axis[]
{
    new Axis
    {
        Labeler = value => new DateTime((long)value).ToString("HH:mm:ss"),
        LabelsRotation = 15,
        UnitWidth = TimeSpan.FromSeconds(1).Ticks,
        MinStep = TimeSpan.FromSeconds(1).Ticks
    }
},
                    YAxes = new Axis[]
                    {
                        new Axis { Name = grupo.Sensores.First().Grandeza ?? "Valor" }
                    }
                });
            }
        }
        
    }
    catch (Exception ex)
    {
        Console.WriteLine("Erro ao carregar grupos: " + ex.Message);
    }
    
}

       public async Task CarregarUltimosSensoresAsync()
{
    try
    {
        var jsonLasts = await client.GetStringAsync("http://localhost:5082/api/sensors/grouped/lasts");

        var ultimosGrupos = JsonSerializer.Deserialize<List<SensorUltimoGrupo>>(jsonLasts, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        SensoresRecentes.Clear();

        foreach (var grupo in ultimosGrupos)
        {
            if (grupo.UltimoSensor != null)
                SensoresRecentes.Add(grupo.UltimoSensor);
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine("Erro ao carregar últimos sensores: " + ex.Message);
    }
}


        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
