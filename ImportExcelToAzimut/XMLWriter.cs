using System.Xml;

namespace ImportExcelToAzimut;

public static class XMLWriter
{
    public static void WriteIntoXmlFromString(string xmlString, string pathToFile)
    {
        XmlDocument xmlDoc = new XmlDocument(); // Создаем новый XmlDocument

        xmlDoc.LoadXml(xmlString); // Загружаем XML из строки

        try  // Сохраняем XML в файл
        {
            xmlDoc.Save(pathToFile);
            Console.WriteLine($"XML файл успешно сохранен в {pathToFile}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при сохранении XML файла: {ex.Message}");
        }
    }
    
    public static void WriteIntoXmlFromString(XmlDocument xmlDoc, string pathToFile)
    {
        try  // Сохраняем XML в файл
        {
            xmlDoc.Save(pathToFile);
            Console.WriteLine($"XML файл успешно сохранен в {pathToFile}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при сохранении XML файла: {ex.Message}");
        }
    }
}