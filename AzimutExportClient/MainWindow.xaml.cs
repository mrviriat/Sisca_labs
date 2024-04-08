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

namespace AzimutExportClient;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        // Создание списка объектов WPT_Route
        List<WPT_Route> routes = new List<WPT_Route>
        {
            new WPT_Route("1", "Маршрут 1"),
            new WPT_Route("2", "Маршрут 2"),
            new WPT_Route("3", "Маршрут 3"),
            new WPT_Route("1", "Маршрут 1"),
            new WPT_Route("2", "Маршрут 2"),
            new WPT_Route("1", "Маршрут 1"),
            new WPT_Route("2", "Маршрут 2"),
            new WPT_Route("1", "Маршрут 1"),
            new WPT_Route("2", "Маршрут 2"),
            new WPT_Route("1", "Маршрут 1"),
            new WPT_Route("2", "Маршрут 2"),
            new WPT_Route("1", "Маршрут 1"),
            new WPT_Route("2", "Маршрут 2"), 
            new WPT_Route("1", "Маршрут 1"),
            new WPT_Route("2", "Маршрут 2"),
            new WPT_Route("1", "Маршрут 1"),
            new WPT_Route("2", "Маршрут 2"),
        };

        // Привязка списка к ListBox
        myListBox.ItemsSource = routes;
    }

    public class WPT_Route
    {
        public string WPT_ID { get; set; }
        public string WPT_NAME { get; set; }

        public WPT_Route(string WPT_ID, string WPT_NAME)
        {
            this.WPT_ID = WPT_ID;
            this.WPT_NAME = WPT_NAME;
        }
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        throw new NotImplementedException();
    }
}