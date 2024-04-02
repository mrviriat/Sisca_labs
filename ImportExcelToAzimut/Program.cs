using System.Text;

namespace ImportExcelToAzimut;

internal class Program
{
    static void Main() //удалить записm под номером 10_000_000 #десять миллионов
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        BDEditor bdEditor = new BDEditor();

        // int itemId = 10_000_001;
        // string xmlString = "<?xml version=\"1.0\" encoding=\"windows-1251\"?>\n<TItinerary active=\"0\" groups=\"14\" route=\"351\" tripcount=\"6\" name=\"name777\" routerun=\"10.4\" maxdtdiff=\"1899-12-30T00-05-00\" maxdtdiffneg=\"1899-12-30T00-03-00\" maxtimeoffset=\"1899-12-30T01-00-00\" recalcmode=\"8\" auxcode=\"auxcodeдо 30.06.2014 910.03.02.12345\" shiftnum=\"2\" departnum=\"3\" usertripcount=\"13\">\n</TItinerary>";

        //bdEditor.AddNewItemToXMLOBJECTSTable(ItemID, xmlString);
        //bdEditor.AddNewItemToRoutsTable(ItemID, "0", "351", "1111", "1111", "1", "1", "0");
        //bdEditor.ReadtFromTbaleByColumnName("XMLOBJECTS", "XMLOBJ_ID", 8_624_780);

        string[] routeNames =
        [
            "КП КПП парка", 
            "БелТАПАЗ", 
            "Производство \"Автоагрегатов\" (Сквер памяти воинов Афганцев)",
            "Технологический колледж (Роддом)", 
            "Улица Советских Пограничников (", 
            "Локомотивное депо (",
            "Университет (",  
            "Детская больница ("
        ]; // Массив значений названий остановок для запроса

        // bdEditor.GetPointsNames(routeNames);

        
        
        List<string> list1 = new List<string>();
        List<string> list2 = new List<string>();

        bdEditor.ReadExcelBook(@"C:\Users\a.gavrilenko\Desktop\15 с 23.03.2024.xls", 
            "разбивка", 
            ref list1,
            ref list2);
        
        // bdEditor.GetPointsNames(list1);
        // bdEditor.GetPointsNames(list2);

        List<int> routesIndexes = bdEditor.ReadExcelForRotes(@"C:\Users\a.gavrilenko\Desktop\6 с 01.03.2024.xls");

        foreach (var index in routesIndexes)
        {
            Console.Write($"{index} ");
        }
    }
}