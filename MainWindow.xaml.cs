using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ReversibleTuringMachine {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {

        public MainWindow() {
            InitializeComponent();
        }

#pragma warning disable S1075
        private void PedroProfileClick(object sender, MouseButtonEventArgs e) {
            Utils.OpenBrowser("https://github.com/PedroRamos360");
        }

        private void PregoProfileClick(object sender, MouseButtonEventArgs e) {
            Utils.OpenBrowser("https://github.com/Pre9o");
        }

        private void RodrigoProfileClick(object sender, MouseButtonEventArgs e) {
            Utils.OpenBrowser("https://github.com/Agentew04");
        }
#pragma warning restore S1075
    }
}