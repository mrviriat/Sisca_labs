using System.Text;
using System.Xml;

namespace ImportExcelToAzimut;

internal class Program
{
    static void Main() //удалить записm под номером 10_000_000 #десять миллионов
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        BdEditor bdEditor = new BdEditor();

        // int itemId = 10_000_001;
        // string xmlString = "<?xml version=\"1.0\" encoding=\"windows-1251\"?>\n<TItinerary active=\"0\" groups=\"14\" route=\"351\" tripcount=\"6\" name=\"name777\" routerun=\"10.4\" maxdtdiff=\"1899-12-30T00-05-00\" maxdtdiffneg=\"1899-12-30T00-03-00\" maxtimeoffset=\"1899-12-30T01-00-00\" recalcmode=\"8\" auxcode=\"auxcodeдо 30.06.2014 910.03.02.12345\" shiftnum=\"2\" departnum=\"3\" usertripcount=\"13\">\n</TItinerary>";

        //bdEditor.AddNewItemToXMLOBJECTSTable(ItemID, xmlString);
        //bdEditor.AddNewItemToRoutsTable(ItemID, "0", "351", "1111", "1111", "1", "1", "0");
        // bdEditor.ReadtFromTbaleByColumnName("XMLOBJECTS", "XMLOBJ_ID", 8_624_780);
        
        
        
        // bdEditor.ReadtFromTbaleByColumnName("XMLOBJECTS", "XMLOBJ_ID", 10_000_014);
       
        

        // bdEditor.GetPointsNames(routeNames);

        
        
        List<string> list1 = new List<string>();
        List<string> list2 = new List<string>();

        // bdEditor.ReadExcelBook(@"C:\Users\a.gavrilenko\Desktop\15 с 23.03.2024.xls", 
        //     "разбивка", 
        //     ref list1,
        //     ref list2);

        string workComputerDirectory = @"C:\Users\a.gavrilenko\Desktop";
        string homeComputerDirectory = @"C:\Users\kazak\OneDrive\Рабочий стол";
        
        bdEditor.ReadExcelBook(homeComputerDirectory + @"\6 с 01.03.2024.xls", 
            "разбивка", 
            ref list1,
            ref list2);

        // int vihodNUmber = 2;
        // int smenaNumber = 1;
        Console.Write("Введите номер выхода, по которому должна быть составлена карточка: ");
        int vihodNUmber = Convert.ToInt32(Console.ReadLine());
        Console.Write("Введите номер смены: ");
        int smenaNumber = Convert.ToInt32(Console.ReadLine());
        
        List<WPT_Route> forwardRoutes = new List<WPT_Route>();
        List<WPT_Route> backwardRoutes = new List<WPT_Route>();
        
        // XmlDocument doc = WriteXMLToAzimut(forwardRoutes, backwardRoutes);
        XmlDocument doc = new XmlDocument();
        
        bdEditor.ReadExcelForAllTimes(homeComputerDirectory + @"\6 с 01.03.2024.xlsx", 
            vihodNUmber, 
            smenaNumber, 
            forwardRoutes, 
            backwardRoutes,
            ref doc
            );
        
        XMLWriter.WriteIntoXmlFromString(doc, homeComputerDirectory + @"\только_что.xml");
        
        string xmlString;
        
        using (StringWriter sw = new StringWriter())
        {
            XmlTextWriter xw = new XmlTextWriter(sw);
            doc.WriteTo(xw);
            xmlString = sw.ToString();
        }
        
        // Console.WriteLine(xmlString);
        return;
        
        // // List<WPT_Route> forwardRoutes = new List<WPT_Route>();
        // // List<WPT_Route> backwardRoutes = new List<WPT_Route>();
        //
        // bdEditor.GetPointsNames(list1, ref forwardRoutes);
        // Console.WriteLine("Конец");
        // Console.WriteLine("Конец");
        // bdEditor.GetPointsNames(list2, ref backwardRoutes);
        //
        // XmlDocument doc = WriteXMLToAzimut(forwardRoutes, backwardRoutes);
        // string xmlString;
        //
        // using (StringWriter sw = new StringWriter())
        // {
        //     XmlTextWriter xw = new XmlTextWriter(sw);
        //     doc.WriteTo(xw);
        //     xmlString = sw.ToString();
        // }
        //
        // // Console.WriteLine(xmlString);
        //
        // int itemId = 10_000_014;
        //
        // // bdEditor.AddNewItemToXmlobjectsTable(itemId, xmlString);
        // // bdEditor.AddNewItemToRoutsTable(
        // //     itemId, 
        // //     "0", 
        // //     "133", 
        // //     "006 кастомный", 
        // //     "6 кастомный", 
        // //     "1", 
        // //     "1", 
        // //     "комментарий");
        //
        // // Console.WriteLine(forwardRoutes.Count);
        // // Console.WriteLine(backwardRoutes.Count);
        //
        // // List<int> routesIndexes = bdEditor.ReadExcelForRotes(@"C:\Users\a.gavrilenko\Desktop\6 с 01.03.2024.xls");
        // //
        // // foreach (var index in routesIndexes)
        // // {
        // //     Console.Write($"{index} ");
        // // }
        // bdEditor.ReadExcelForAllTimes(@"C:\Users\a.gavrilenko\Desktop\6 с 01.03.2024.xlsx", vihodNUmber, smenaNumber);
    }
    
    static XmlDocument WriteXMLToAzimut(List<WPT_Route> forwardRoutes, List<WPT_Route> backwardRoutes)
    {
        XmlDocument doc = new XmlDocument();
        
        XmlDeclaration xmlDeclaration = doc.CreateXmlDeclaration("1.0", "WINDOWS-1251", null);
        doc.AppendChild(xmlDeclaration);
        
        XmlElement root = doc.CreateElement("TItinerary");
        root.SetAttribute("departnum", "1");
        root.SetAttribute("shiftnum", "1");
        
        root.SetAttribute("recalcmode", "8");
        root.SetAttribute("maxtimeoffset", "1899-12-30T01-00-00");
        root.SetAttribute("maxdtdiffneg", "1899-12-30T00-03-00");
        root.SetAttribute("maxdtdiff", "1899-12-30T00-05-00");
        root.SetAttribute("routerun", "3.7");
        
        root.SetAttribute("usertripcount", "6");
        
        root.SetAttribute("name", "006 кастомный");
        root.SetAttribute("auxcode", "006 кастомный");
        root.SetAttribute("tripcount", "6");
        root.SetAttribute("route", "133");
        root.SetAttribute("groups", "6");
        
        XmlElement rp = doc.CreateElement("rp");
        
        XmlElement firstPoint = doc.CreateElement("TRoutePoint");
        firstPoint.SetAttribute("wptid", "15");
        firstPoint.SetAttribute("name", "КП КПП парка");
        firstPoint.SetAttribute("run", "0");
        firstPoint.SetAttribute("usertripnew", "1");
        firstPoint.SetAttribute("id", "1");
        rp.AppendChild(firstPoint);

        for (int i = 0; i < forwardRoutes.Count; i++)
        {
            WPT_Route route = forwardRoutes[i];
            
            XmlElement forwardPoint = doc.CreateElement("TRoutePoint");
            forwardPoint.SetAttribute("wptid", route.WPT_ID);
            forwardPoint.SetAttribute("name", route.WPT_NAME);
            forwardPoint.SetAttribute("run", "0");
            forwardPoint.SetAttribute("id", $"{i + 2}");

            if (i == 0)
            {
                forwardPoint.SetAttribute("usertripnew", "1");
            }
            
            rp.AppendChild(forwardPoint);
        }
        
        for (int i = 0; i < backwardRoutes.Count; i++)
        {
            WPT_Route route = backwardRoutes[i];
            
            XmlElement backwardPoint = doc.CreateElement("TRoutePoint");
            backwardPoint.SetAttribute("wptid", route.WPT_ID);
            backwardPoint.SetAttribute("name", route.WPT_NAME);
            backwardPoint.SetAttribute("run", "0");
            backwardPoint.SetAttribute("id", $"{i + 2 + forwardRoutes.Count}");

            if (i == 0)
            {
                backwardPoint.SetAttribute("usertripnew", "1");
            }
            
            rp.AppendChild(backwardPoint);
        }

        root.AppendChild(rp);

        XmlElement tp = doc.CreateElement("tp");
        
    

        string timeString = "1899-12-30T06-50-00";
        
        for (int i = 0; i < 6; i++)
        {
            for (int j = 2; j <= 47; j++)
            {
                DateTime time = DateTime.ParseExact(timeString, "yyyy-MM-ddTHH-mm-ss", null);
                time = time.AddMinutes(2);
                timeString = time.ToString("yyyy-MM-ddTHH-mm-ss");

                XmlElement newpoint = doc.CreateElement("TTimePoint");
                // newpoint.SetAttribute("usertripnew", "1");
                newpoint.SetAttribute("routepnt", j.ToString());
                newpoint.SetAttribute("trip", i.ToString());
                newpoint.SetAttribute("time", timeString);
                tp.AppendChild(newpoint);
            }
        }
        
        // XmlElement newpoint = doc.CreateElement("TTimePoint");
        // // newpoint.SetAttribute("usertripnew", "1");
        // newpoint.SetAttribute("routepnt", "7");
        // newpoint.SetAttribute("trip", "0");
        // newpoint.SetAttribute("time", "1899-12-30T06-32-00");
        // tp.AppendChild(newpoint);
        //
        // XmlElement newpoint3 = doc.CreateElement("TTimePoint");
        // // newpoint.SetAttribute("usertripnew", "1");
        // newpoint3.SetAttribute("routepnt", "26");
        // newpoint3.SetAttribute("trip", "0");
        // newpoint3.SetAttribute("time", "1899-12-30T06-40-00");
        // tp.AppendChild(newpoint3);
        //
        //
        //
        // XmlElement newpoint2 = doc.CreateElement("TTimePoint");
        // // newpoint2.SetAttribute("usertripnew", "2");
        // newpoint2.SetAttribute("routepnt", "9");
        // newpoint2.SetAttribute("trip", "3");
        // newpoint2.SetAttribute("time", "1899-12-30T06-50-00");
        // tp.AppendChild(newpoint2);
        
        root.AppendChild(tp);

        doc.AppendChild(root);

        // XMLWriter.WriteIntoXmlFromString(doc, @"C:\Users\a.gavrilenko\Desktop\FileWith006Data.xml");
        
        return doc;
    }
}