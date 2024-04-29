using System.IO;
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
using System.Xml;
using Microsoft.Win32;

namespace AzimutExportClient;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    
    public MainWindow()
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        
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
        routesForBuildingTemplate.ItemsSource = routes;
        
    }

    

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        throw new NotImplementedException();
    }

    public string FilePath = "";
    
    private void OpenFile_Click(object sender, RoutedEventArgs e)
    {
        OpenFileDialog openFileDialog = new OpenFileDialog();
        openFileDialog.Filter = "All files (*.*)|*.*";
        if (openFileDialog.ShowDialog() == true)
        {
            FilePath = openFileDialog.FileName;
            Console.WriteLine(FilePath);
            FilePathTextBlock.Text = FilePath;
        }
    }
    
    private void startCreatePass(object sender, RoutedEventArgs e)
    {
        var bdEditor = new BdEditor();
        ListsGrid.Opacity = 1;
        ListsGrid.IsHitTestVisible = true;
        string request = "SELECT WPT_ID, WPT_NAME, WPT_TAG FROM WPT WHERE WPT_TAG = 1 ORDER BY WPT_NAME";
        List<WPT_Route> allRoutes = bdEditor.ReadtFromTbaleWithCustomRequest(request);
        allRoutesFromDataBase.ItemsSource = allRoutes;
        
        
        
        List<string> list1 = new List<string>();
        List<string> list2 = new List<string>();

        string workComputerDirectory = @"C:\Users\a.gavrilenko\Desktop";
        string homeComputerDirectory = @"C:\Users\kazak\OneDrive\Рабочий стол";
        
        bdEditor.ReadExcelBook(workComputerDirectory + @"\6 с 01.03.2024.xls", 
            "разбивка", 
            ref list1,
            ref list2);

        // int vihodNUmber = 2;
        // int smenaNumber = 1;
        Console.Write("Введите номер выхода, по которому должна быть составлена карточка: ");
        int vihodNUmber = Convert.ToInt32(Console.ReadLine());
        Console.Write("Введите номер смены: ");
        int smenaNumber = Convert.ToInt32(Console.ReadLine());
        Console.Write("Введите название карточки: ");
        string cardName = Console.ReadLine();
        
        List<WPT_Route> forwardRoutes = new List<WPT_Route>();
        List<WPT_Route> backwardRoutes = new List<WPT_Route>();
        
        bdEditor.GetPointsNames(list1, ref forwardRoutes);
        bdEditor.GetPointsNames(list2, ref backwardRoutes);
        // XmlDocument doc = WriteXMLToAzimut(forwardRoutes, backwardRoutes);
        XmlDocument doc = new XmlDocument();
        
        bdEditor.ReadExcelForAllTimes(
            workComputerDirectory + @"\6 с 01.03.2024.xlsx", 
            vihodNUmber, 
            smenaNumber, 
            cardName,
            forwardRoutes, 
            backwardRoutes,
            ref doc
            );
        
        // XMLWriter.WriteIntoXmlFromString(doc, workComputerDirectory + @"\только_что.xml");
        
        string xmlString;
        
        using (StringWriter sw = new StringWriter())
        {
            XmlTextWriter xw = new XmlTextWriter(sw);
            doc.WriteTo(xw);
            xmlString = sw.ToString();
        }
        
        string filePath = workComputerDirectory +@"\numbers.txt";
        int itemId = ReadNumberFromFile(filePath);
        
        // int itemId = 10_000_017;
        
        bdEditor.AddNewItemToXmlobjectsTable(itemId, xmlString);
        bdEditor.AddNewItemToRoutsTable(
            itemId, 
            "0", 
            "133", 
            cardName, 
            "6 кастомный", 
            $"{smenaNumber}", 
            $"{vihodNUmber}", 
            "комментарий");
        
        WriteNumberToFile(filePath, itemId + 2);
    }
    
    
    static int ReadNumberFromFile(string filePath)
    {
        int number;
        try
        {
            // Чтение числа из файла
            using (StreamReader sr = new StreamReader(filePath))
            {
                number = int.Parse(sr.ReadLine());
            }
        }
        catch (FileNotFoundException)
        {
            Console.WriteLine("Файл не найден. Введите число:");
            number = int.Parse(Console.ReadLine());
            WriteNumberToFile(filePath, number); // Запись в файл, если он не был найден
        }
        return number;
    }
    
    static void WriteNumberToFile(string filePath, int number)
    {
        // Запись числа в файл
        using (StreamWriter sw = new StreamWriter(filePath))
        {
            sw.WriteLine(number);
        }
    }
}