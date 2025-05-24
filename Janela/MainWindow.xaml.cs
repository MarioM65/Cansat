using System.Windows;

namespace Janela
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnClique_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Você clicou no botão!");
        }
    }
}
