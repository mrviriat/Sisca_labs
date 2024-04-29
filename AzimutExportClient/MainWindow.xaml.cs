using System.IO;
using System.Text;
using System.Windows;
using System.Xml;
using Microsoft.Win32;

namespace AzimutExportClient;

public partial class MainWindow : Window
{
    private readonly BdEditor _bdEditor = new BdEditor();
    private string _filePath = "";
    public MainWindow()
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        
        InitializeComponent();
    }
    
    private void OpenFile_Click(object sender, RoutedEventArgs e)
    {
        OpenFileDialog openFileDialog = new OpenFileDialog();
        openFileDialog.Filter = "All files (*.*)|*.*";
        if (openFileDialog.ShowDialog() == true)
        {
            _filePath = openFileDialog.FileName;
            FilePathTextBlock.Text = _filePath;
        }
    }
    
    private void StartCreatePass(object sender, RoutedEventArgs e)
    {
        ListsGrid.Opacity = 1;
        ListsGrid.IsHitTestVisible = true;
        
        List<WPT_Route> allRoutes = _bdEditor.GetAllWptPoints();
        allRoutesFromDataBase.ItemsSource = allRoutes;
        
        
        List<string> forwardWptListFromExcel = new List<string>();
        List<string> backwardWptListFromExcel = new List<string>();

        var workComputerDirectory = @"C:\Users\a.gavrilenko\Desktop";
        var homeComputerDirectory = @"C:\Users\kazak\OneDrive\Рабочий стол";
        
        _bdEditor.ReadExcelBook(
            workComputerDirectory + @"\6 с 01.03.2024.xls", 
            "разбивка", 
            ref forwardWptListFromExcel,
            ref backwardWptListFromExcel);

        int vihodNUmber = Convert.ToInt32(VihodNUmber.Text);
        int smenaNumber = Convert.ToInt32(SmenaNumber.Text);
        string daysType = DaysTypeComboBox.SelectedIndex == 0 ? "12345" : "67";
        string cardName = $"Курсовая работа 006.0{vihodNUmber}.0{smenaNumber}.{daysType}";
        
        List<WPT_Route> finalForwardRoutesList = new List<WPT_Route>();
        List<WPT_Route> finalBackwardRoutesList = new List<WPT_Route>();
        
        _bdEditor.GetPointsNames(forwardWptListFromExcel, ref finalForwardRoutesList);
        routesForBuildingTemplate.Items.Add(new WPT_Route("-->", "Прямое следование"));
        foreach (var element in finalForwardRoutesList)
        {
            routesForBuildingTemplate.Items.Add(element);
        }
        
        _bdEditor.GetPointsNames(backwardWptListFromExcel, ref finalBackwardRoutesList);
        routesForBuildingTemplate.Items.Add(new WPT_Route("--<", "Обратное следование"));
        foreach (var element in finalBackwardRoutesList)
        {
            routesForBuildingTemplate.Items.Add(element);
        }
        
        var xmlDoc = new XmlDocument();
        
        _bdEditor.ReadExcelForAllTimes(
            workComputerDirectory + @"\6 с 01.03.2024.xlsx", 
            vihodNUmber, 
            smenaNumber, 
            cardName,
            finalForwardRoutesList, 
            finalBackwardRoutesList,
            ref xmlDoc);
        
        string xmlString;
        
        using (var sw = new StringWriter())
        {
            var xw = new XmlTextWriter(sw);
            xmlDoc.WriteTo(xw);
            xmlString = sw.ToString();
        }
        
        string filePath = workComputerDirectory + @"\numbers.txt";
        int itemId = ReadNumberFromFile(filePath);
        
        _bdEditor.AddNewItemToXmlobjectsTable(itemId, xmlString);
        _bdEditor.AddNewItemToRoutsTable(
            itemId, 
            "0", 
            "133", 
            cardName, 
            cardName, 
            $"{smenaNumber}", 
            $"{vihodNUmber}", 
            "комментарий");
        
        WriteNumberToFile(filePath, itemId + 2);
    }
    
    static int ReadNumberFromFile(string filePath)
    {
        int number;
        using StreamReader sr = new StreamReader(filePath);
        number = int.Parse(sr.ReadLine());
        return number;
    }
    
    static void WriteNumberToFile(string filePath, int number)
    {
        using StreamWriter sw = new StreamWriter(filePath);
        sw.WriteLine(number);
    }
}