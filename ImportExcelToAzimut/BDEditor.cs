using FirebirdSql.Data.FirebirdClient;
using System;
using ExcelDataReader;
using OfficeOpenXml;
using System.Data;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;

namespace ImportExcelToAzimut;

public class BdEditor
{
    static string _configurationData =
        "database=azimut;User=azimut;Password=_azimut#;Role=RL1;Dialect=3;Server=192.168.1.112;Port=3050;Charset=WIN1251";

    public void AddNewItemToXmlobjectsTable(int itemId, string xmlString)
    {
        using (FbConnection connection = new FbConnection(_configurationData))
        {
            connection.Open();

            FbTransaction transaction = connection.BeginTransaction();
            try
            {
                string insertQuery =
                    "INSERT INTO XMLOBJECTS (XMLOBJ_ID, XMLOBJ_XMLCLS_ID, XMLOBJ_XMLCLS_NAME, XMLOBJ_DELETED, XMLOBJ_CHNG_DT, XMLOBJ_CREATE_DT, XMLOBJ_WHO, XMLOBJ_TRIGGEROFF, XMLOBJ_XML) " +
                    "VALUES (@Value1, @Value2, @Value3, @Value4, @Value5, @Value6, @Value7, @Value8, @Value9)"; // Создаем команду для выполнения SQL-запроса INSERT INTO

                FbCommand command = new FbCommand(insertQuery, connection, transaction);

                command.Parameters.AddWithValue("@Value1", itemId); // Добавляем параметры для каждого значения
                command.Parameters.AddWithValue("@Value2", 1);
                command.Parameters.AddWithValue("@Value3", "TItinerary");
                command.Parameters.AddWithValue("@Value4", 0);
                command.Parameters.AddWithValue("@Value5", "21.03.2024 12:40:05"); // Изменить на текущаю дату
                command.Parameters.AddWithValue("@Value6", "21.03.2024 12:40:05");
                command.Parameters.AddWithValue("@Value7", "AZ_USER");
                command.Parameters.AddWithValue("@Value8", 0);
                command.Parameters.AddWithValue("@Value9", xmlString);

                command.ExecuteNonQuery(); // Выполняем запрос

                transaction.Commit(); // Коммитим транзакцию

                Console.WriteLine($"Новый элемент c ID, равным {itemId}, успешно добавлен в таблицу XMLOBJECTS.");
            }
            catch (Exception ex) // Если возникла ошибка, откатываем транзакцию
            {
                transaction.Rollback();
                Console.WriteLine(
                    $"Ошибка при добавлении нового элемента c ID, равным {itemId}, в таблицу XMLOBJECTS: " +
                    ex.Message);
            }
        }
    }

    public void AddNewItemToRoutsTable(int itemId, string newItemValue2, string newItemValue3,
        string newItemValue4, string newItemValue6, string newItemValue7, string newItemValue8, string newItemValue9)
    {
        using (FbConnection connection = new FbConnection(_configurationData))
        {
            connection.Open();

            FbTransaction transaction = connection.BeginTransaction();
            try
            {
                string insertQuery =
                    "INSERT INTO ITINERARIES (ITIN_XMLOBJ_ID, ITIN_ACTIVE, ITIN_ROUTE_ID, ITIN_NAME, ITIN_SHIFTGROUP, ITIN_AUXCODE, ITIN_SHIFTNUM, ITIN_DEPARTNUM, ITIN_COMMENT) " +
                    "VALUES (@Value1, @Value2, @Value3, @Value4, @Value5, @Value6, @Value7, @Value8, @Value9)"; // Создаем команду для выполнения SQL-запроса INSERT INTO

                FbCommand command = new FbCommand(insertQuery, connection, transaction);

                command.Parameters.AddWithValue("@Value1", itemId); // Добавляем параметры для каждого значения
                command.Parameters.AddWithValue("@Value2", newItemValue2);
                command.Parameters.AddWithValue("@Value3", newItemValue3);
                command.Parameters.AddWithValue("@Value4", newItemValue4);
                command.Parameters.AddWithValue("@Value5", null);
                command.Parameters.AddWithValue("@Value6", newItemValue6);
                command.Parameters.AddWithValue("@Value7", newItemValue7);
                command.Parameters.AddWithValue("@Value8", newItemValue8);
                command.Parameters.AddWithValue("@Value9", newItemValue9);

                command.ExecuteNonQuery(); // Выполняем запрос

                transaction.Commit(); // Коммитим транзакцию

                Console.WriteLine($"Новый элемент c ID, равным {itemId}, успешно добавлен в таблицу ITINERARIES.");
            }
            catch (Exception ex) // Если возникла ошибка, откатываем транзакцию
            {
                transaction.Rollback();
                Console.WriteLine(
                    $"Ошибка при добавлении нового элемента c ID, равным {itemId}, в таблицу ITINERARIES: " +
                    ex.Message);
            }
        }
    }

    public void ReadtFromTbaleByColumnName(string tableName, string columnName, int columnValue)
    {
        using (FbConnection connection = new FbConnection(_configurationData))
        {
            connection.Open();

            FbTransaction transaction = connection.BeginTransaction();
            try
            {
                string insertQuery =
                    $"SELECT * FROM {tableName} WHERE {columnName} = @ColumnValue"; //SQL запрос в базу данных

                FbCommand command = new FbCommand(insertQuery, connection, transaction);
                command.Parameters.AddWithValue("@ColumnValue", columnValue);

                FbDataReader reader = command.ExecuteReader();

                while (reader.Read()) // Выводим значения всех столбцов данной строки
                {
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        Console.WriteLine($"{reader.GetName(i)}: {reader[i]}");
                    }

                    XMLWriter.WriteIntoXmlFromString((string)reader[8],
                        @"C:\Users\a.gavrilenko\Desktop\FileWith14Data.xml");
                }

                reader.Close();
            }
            catch (Exception ex) // Если возникла ошибка, откатываем транзакцию
            {
                transaction.Rollback();
                Console.WriteLine(
                    $"Ошибка при чтении записи со значением столбца {columnName}, равным {columnValue}, в таблице {tableName}: " +
                    ex.Message);
            }
        }
    }

    public void GetPointsNames(List<string> routeNames, ref List<WPT_Route> finishRoutes)
    {
        // using (FbConnection connection = new FbConnection(_configurationData))
        // {
        //     connection.Open();
        //
        //     FbTransaction transaction = connection.BeginTransaction();
        //     try
        //     {
        //         string query = "SELECT * FROM WPT WHERE WPT_NAME IN (";
        //         
        //         for (int i = 0; i < routeNames.Length; i++)
        //         {
        //             query += "@RouteName" + i;
        //             if (i < routeNames.Length - 1)
        //                 query += ",";
        //         }
        //         
        //         query += ")";
        //
        //         using (FbCommand command = new FbCommand(query, connection, transaction))
        //         {
        //             for (int i = 0; i < routeNames.Length; i++)
        //             {
        //                 command.Parameters.AddWithValue("@RouteName" + i, routeNames[i]);
        //             }
        //
        //             using (FbDataReader reader = command.ExecuteReader())
        //             {
        //                 while (reader.Read())
        //                 {
        //                     Console.WriteLine("WPT_ID: {0}, WPT_NAME: {1}",  // Обработка результатов запроса
        //                         reader["WPT_ID"], reader["WPT_NAME"]);
        //                 }
        //             }
        //         }
        //     }
        //     catch (Exception ex) // Если возникла ошибка, откатываем транзакцию
        //     {
        //         transaction.Rollback();
        //         Console.WriteLine($"Ошибка при чтении записией: " + ex.Message);
        //     }
        // }

        using (FbConnection connection = new FbConnection(_configurationData))
        {
            connection.Open();

            FbTransaction transaction = connection.BeginTransaction();
            try
            {
                // // Создаем строку-шаблон для оператора LIKE
                // string parameterPlaceholders = string.Join(" OR ", routeNames.Select((s, i) => "WPT_NAME LIKE @p" + i));
                //
                // // Создаем команду для выполнения SQL-запроса с использованием оператора LIKE
                // FbCommand cmdp = new FbCommand($"SELECT * FROM WPT WHERE {parameterPlaceholders}", fbtr, fbTransop);
                //
                //
                // for (int i = 0; i < routeNames.Length; i++)
                // {
                //     cmdp.Parameters.AddWithValue($"@p{i}", $"%{routeNames[i]}%");
                // }

                for (int i = 0; i < routeNames.Count; i++)
                {
                    string searchRoute;

                    if (i == routeNames.Count - 1)
                    {
                        searchRoute = routeNames[i];
                    }
                    else
                    {
                        string pattern = @"\s*\([^)]*\)";

                        string routeNameWithoutLastPart =
                            Regex.Replace(routeNames[i + 1], pattern, ""); // Удаляем найденный текст

                        searchRoute = $"{routeNames[i]} ({routeNameWithoutLastPart})";
                    }

                    // Создаем команду для выполнения SQL-запроса с использованием оператора LIKE
                    FbCommand command = new FbCommand("SELECT * FROM WPT WHERE WPT_NAME LIKE @searchRoute", connection,
                        transaction);

                    command.Parameters.AddWithValue("@searchRoute", $"%{searchRoute}%");


                    string result = "";

                    using (FbDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (Convert.ToInt32(reader["WPT_ID"]) == 473)
                            {
                                continue;
                            }

                            result = $"WPT_ID: {reader["WPT_ID"]}, WPT_NAME: {reader["WPT_NAME"]}";
                            Console.WriteLine(result);
                            finishRoutes.Add(new WPT_Route(Convert.ToString(reader["WPT_ID"]),
                                Convert.ToString(reader["WPT_NAME"])));

                            // Console.WriteLine("WPT_ID: {0}, WPT_NAME: \"{1}\"", reader["WPT_ID"], reader["WPT_NAME"]);  // Выводим значения всех столбцов данной строки
                        }

                        if (result != "")
                        {
                            Console.WriteLine("===============================");
                        }
                    }

                    if (result == "")
                    {
                        string searchRoutee = routeNames[i];
                        FbCommand commandd = new FbCommand("SELECT * FROM WPT WHERE WPT_NAME LIKE @searchRoutee",
                            connection, transaction);

                        commandd.Parameters.AddWithValue("@searchRoutee", $"%{searchRoutee}%");

                        using (FbDataReader readerr = commandd.ExecuteReader())
                        {
                            while (readerr.Read())
                            {
                                if (Convert.ToInt32(readerr["WPT_ID"]) == 473)
                                {
                                    continue;
                                }

                                Console.WriteLine("WPT_ID: {0}, WPT_NAME: \"{1}\"", readerr["WPT_ID"],
                                    readerr["WPT_NAME"]); // Выводим значения всех столбцов данной строки
                                finishRoutes.Add(new WPT_Route(Convert.ToString(readerr["WPT_ID"]),
                                    Convert.ToString(readerr["WPT_NAME"])));
                            }

                            Console.WriteLine("===============================");
                        }
                    }
                }

                // foreach (string searchRoute in routeNames)
                // {
                //     // Создаем команду для выполнения SQL-запроса с использованием оператора LIKE
                //     FbCommand command = new FbCommand("SELECT * FROM WPT WHERE WPT_NAME LIKE @searchRoute", connection, transaction);
                //     
                //     command.Parameters.AddWithValue("@searchRoute", $"%{searchRoute}%");
                //
                //     using (FbDataReader reader = command.ExecuteReader())
                //     {
                //         while (reader.Read())
                //         {
                //             Console.WriteLine("WPT_ID: {0}, WPT_NAME: \"{1}\"", reader["WPT_ID"], reader["WPT_NAME"]);  // Выводим значения всех столбцов данной строки
                //         }
                //
                //         Console.WriteLine("===============================");
                //     }
                // }
            }
            finally
            {
                transaction.Commit(); // Завершаем транзакцию
            }
        }
    }

    public void ReadExcelBook(string filePath, string sheetName, ref List<string> list1, ref List<string> list2)
    {
        using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
        {
            using (var reader = ExcelReaderFactory.CreateReader(stream))
            {
                reader.NextResult();
                reader.NextResult();

                while (reader.Read()) // Читаем построчно
                {
                    // Начинаем с третьей строки, игнорируя первые две
                    if (reader.Depth >= 2)
                    {
                        if (reader[0] != null && !string.IsNullOrWhiteSpace(reader.GetString(0)))
                        {
                            string route = reader.GetString(0).TrimEnd();
                            // Console.WriteLine($"'{route}'"); // Выводим данные строки в консоль
                            list1.Add(route);
                        }
                        else // Если строка пустая, прерываем цикл
                        {
                            break;
                        }
                    }
                }

                reader.Read();

                while (reader.Read()) // Читаем построчно
                {
                    if (reader[0] != null && !string.IsNullOrWhiteSpace(reader.GetString(0)))
                    {
                        string route = reader.GetString(0).TrimEnd();
                        // Console.WriteLine($"'{route}''"); // Выводим данные строки в консоль
                        list2.Add(route);
                    }
                    else // Если строка пустая, прерываем цикл
                    {
                        break;
                    }
                }
            }
        }
    }

    public List<int> ReadExcelForRotes(string filePath)
    {
        List<int> routesIndexes = new List<int>();

        using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
        {
            using (var reader = ExcelReaderFactory.CreateReader(stream))
            {
                while (reader.Read()) // Читаем построчно
                {
                    if (reader.GetString(1) == "АП г. Гродно")
                    {
                        routesIndexes.Add(reader.Depth + 1);
                        Console.WriteLine($"{reader.Depth + 1}: {reader.GetString(1)}");
                    }
                }
            }
        }

        return routesIndexes;
    }

    private string ConvertStringToDate(string timeString)
    {
        string[] timeParts = timeString.Split(':'); // Разделение строки на часы и минуты

        int hour = int.Parse(timeParts[0]);
        string hours = timeParts[0]; // Получение часов и минут из строки
        string minutes = timeParts[1];
        
        if (hours.Length == 1)
        {
            hours = $"0{hours}";
        }

        string time;

        if (hour < 4) // Создание объекта DateTime с датой 30/31 декабря 1899 года и указанием времени
        {
            time = $"1899-12-31T{hours}-{minutes}-00";
        }
        else
        {
            time = $"1899-12-30T{hours}-{minutes}-00";
        }

        return time;
    }


    private int CalculateTimeDifferenceInMinutes(string timeString1, string timeString2)
    {
        // Разделение строк на часы и минуты
        string[] timeParts1 = timeString1.Split(':');
        string[] timeParts2 = timeString2.Split(':');

        // Получение часов и минут из строк
        int hours1 = int.Parse(timeParts1[0]);
        int minutes1 = int.Parse(timeParts1[1]);
        int hours2 = int.Parse(timeParts2[0]);
        int minutes2 = int.Parse(timeParts2[1]);

        // Создание объектов DateTime с датой 30 декабря 1899 года и указанием времени
        DateTime time1 = new DateTime(1899, 12, 30, hours1, minutes1, 0);
        DateTime time2 = new DateTime(1899, 12, 30, hours2, minutes2, 0);

        // Вычисление разницы в минутах
        TimeSpan difference = time2 - time1;

        return (int)difference.TotalMinutes;
    }
    
    
    // главная функция по сути в которой происходит сборка всего
    public void ReadExcelForAllTimes(string filePath, int vihodNumber, int smenaNumber, List<WPT_Route> forwardRoutes, List<WPT_Route> backwardRoutes, ref XmlDocument doc)
    {
        
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
        
        XmlElement lastPoint = doc.CreateElement("TRoutePoint");
        lastPoint.SetAttribute("wptid", "15");
        lastPoint.SetAttribute("name", "КП КПП парка");
        lastPoint.SetAttribute("run", "0");
        lastPoint.SetAttribute("id", $"{1 + forwardRoutes.Count + backwardRoutes.Count + 1}");
        rp.AppendChild(lastPoint);

        root.AppendChild(rp);

        XmlElement tp = doc.CreateElement("tp");





        List<int> viezdZezdVGaragePoVihodam = ReadExcelForRotes(filePath);
        List<int> parkEndTimeRows = new List<int>();

        using (ExcelPackage package = new ExcelPackage(new FileInfo(filePath))) // Открываем файл Excel с помощью EPPlus
        {
            ExcelWorksheet worksheet = package.Workbook.Worksheets[0]; // Получаем первый лист в книге

            int row = vihodNumber * 2 - 1;

            for (int i = 1; i < 50; i++)
            {
                if (worksheet.Cells[viezdZezdVGaragePoVihodam[row], i].Text.Length > 0 &&
                    worksheet.Cells[viezdZezdVGaragePoVihodam[row], i].Text.Length <= 5)
                {
                    parkEndTimeRows.Add(i);
                    Console.WriteLine(worksheet.Cells[viezdZezdVGaragePoVihodam[row], i].Text);
                    Console.WriteLine($"Строка - {viezdZezdVGaragePoVihodam[row]}");
                    Console.WriteLine($"Колонка - {i}");
                }
            }
        }

        using (ExcelPackage package = new ExcelPackage(new FileInfo(filePath))) // Открываем файл Excel с помощью EPPlus
        {
            ExcelWorksheet worksheet = package.Workbook.Worksheets[0]; // Получаем первый лист в книге
            ExcelWorksheet worksheetRazbivka = package.Workbook.Worksheets[2];

            int indexOfRowOfTimeStart = vihodNumber * 2 - 2;
            int indexOfRowOfTimeEnd = vihodNumber * 2 - 1;

            int middleRow = (viezdZezdVGaragePoVihodam[indexOfRowOfTimeEnd] -
                             viezdZezdVGaragePoVihodam[indexOfRowOfTimeStart]) / 2 +
                            viezdZezdVGaragePoVihodam[indexOfRowOfTimeStart];

            bool findSeconSmena = false;
            int schetchickColonokVAzimut = 0;

            if (smenaNumber == 1)
            {
                string firsttimestring = ConvertStringToDate(worksheet.Cells[viezdZezdVGaragePoVihodam[indexOfRowOfTimeStart], 3].Text);
                XmlElement firstpoint = doc.CreateElement("TTimePoint");
                firstpoint.SetAttribute("routepnt", "1");
                firstpoint.SetAttribute("trip", "0");
                firstpoint.SetAttribute("time", firsttimestring);
                tp.AppendChild(firstpoint);
            }
            // parkEndTimeRows это номера колонок, в которых автобус в исследуемом выходе заезжал в гараж. 
            // пока что код рассчитан на один заезд в гараж (parkEndTimeRows[0])
            for (int col = 3; col <= parkEndTimeRows[0]; col++)
            {
                if (smenaNumber == 1) // случай, когда нужна первая смена
                {
                    if (worksheet.Cells[viezdZezdVGaragePoVihodam[indexOfRowOfTimeStart] + 1, col].Text == "обед")
                    {
                        continue;
                    }

                    int firstStart = viezdZezdVGaragePoVihodam[indexOfRowOfTimeStart] + 1;
                    for (int row = firstStart; row < middleRow; row++)
                    {
                        Console.WriteLine(ConvertStringToDate(worksheet.Cells[row, col].Text));

                        // if (worksheet.Cells[row, col].Style.Font.UnderLine)
                        // {
                        //     Console.WriteLine("Конец первой смены");
                        // }
                    }

                    int firstDiff = CalculateTimeDifferenceInMinutes(worksheet.Cells[firstStart, col].Text,
                        worksheet.Cells[middleRow - 1, col].Text);
                    int forwardCollNumberInRazbivka = 0;
                    for (int i = 2; i <= 4; i++)
                    {
                        if (firstDiff.ToString() == worksheetRazbivka.Cells[26, i].Text)
                        {
                            forwardCollNumberInRazbivka = i;
                            break;
                        }
                    }

                    Console.WriteLine(
                        $"продолжительность рейса = {firstDiff}, смотрим разбивку по {forwardCollNumberInRazbivka} колонке");

                    string timeString = ConvertStringToDate(worksheet.Cells[firstStart, col].Text);
                    for (int i = 3; i <= 26; i++)
                    {
                        DateTime time = DateTime.ParseExact(timeString, "yyyy-MM-ddTHH-mm-ss", null);
                        time = time.AddMinutes(Convert.ToInt32(worksheetRazbivka.Cells[i, forwardCollNumberInRazbivka].Text));
                        string tempTime = time.ToString("yyyy-MM-ddTHH-mm-ss");

                        XmlElement newpoint = doc.CreateElement("TTimePoint");
                        // newpoint.SetAttribute("usertripnew", "1");
                        newpoint.SetAttribute("routepnt", $"{i-1}");
                        newpoint.SetAttribute("trip", $"{schetchickColonokVAzimut}");
                        newpoint.SetAttribute("time", tempTime);
                        tp.AppendChild(newpoint);
                    }
                    
                    if (worksheet.Cells[middleRow - 1, col].Style.Font.UnderLine)
                    {
                        Console.WriteLine("Конец первой смены");
                        break;
                    }


                    Console.WriteLine();


                    int secondStart = middleRow + 1;
                    for (int row = secondStart; row < viezdZezdVGaragePoVihodam[indexOfRowOfTimeEnd]; row++)
                    {
                        Console.WriteLine(ConvertStringToDate(worksheet.Cells[row, col].Text));

                        // if (worksheet.Cells[row, col].Style.Font.UnderLine)
                        // {
                        //     Console.WriteLine("Конец первой смены");
                        // }
                    }

                    int secondDiff = CalculateTimeDifferenceInMinutes(worksheet.Cells[secondStart, col].Text,
                        worksheet.Cells[viezdZezdVGaragePoVihodam[indexOfRowOfTimeEnd] - 1, col].Text);
                    int backwardCollNumberInRazbivka = 0;
                    for (int i = 2; i <= 4; i++)
                    {
                        if (secondDiff.ToString() == worksheetRazbivka.Cells[50, i].Text)
                        {
                            backwardCollNumberInRazbivka = i;
                            break;
                        }
                    }

                    Console.WriteLine(
                        $"продолжительность обратного рейса = {secondDiff}, смотрим разбивку по {backwardCollNumberInRazbivka} колонке");

                    timeString = ConvertStringToDate(worksheet.Cells[secondStart, col].Text);
                    for (int i = 29; i <= 50; i++)  // заменить индексы на такие, чтобы программа сама считала
                    {
                        DateTime time = DateTime.ParseExact(timeString, "yyyy-MM-ddTHH-mm-ss", null);
                        time = time.AddMinutes(Convert.ToInt32(worksheetRazbivka.Cells[i, backwardCollNumberInRazbivka].Text));
                        string tempTime = time.ToString("yyyy-MM-ddTHH-mm-ss");

                        XmlElement newpoint = doc.CreateElement("TTimePoint");
                        // newpoint.SetAttribute("usertripnew", "1");
                        newpoint.SetAttribute("routepnt", $"{i-3}");
                        newpoint.SetAttribute("trip", $"{schetchickColonokVAzimut}");
                        newpoint.SetAttribute("time", tempTime);
                        tp.AppendChild(newpoint);
                    }
                    if (worksheet.Cells[viezdZezdVGaragePoVihodam[indexOfRowOfTimeEnd] - 1, col].Style.Font.UnderLine)
                    {
                        Console.WriteLine("Конец первой смены");
                        break;
                    }

                    schetchickColonokVAzimut += 1;
                    Console.WriteLine();
                }
                else // случай, когда нам нужна вторая смена
                {
                    if (worksheet.Cells[viezdZezdVGaragePoVihodam[indexOfRowOfTimeStart] + 1, col].Text == "обед")
                    {
                        continue;
                    }

                    if ( // если и прямой и обратный рейсы принадлежат первой смене
                        !findSeconSmena &&
                        !worksheet.Cells[middleRow - 1, col].Style.Font.UnderLine &&
                        !worksheet.Cells[viezdZezdVGaragePoVihodam[indexOfRowOfTimeEnd] - 1, col].Style.Font.UnderLine
                       )
                    {
                        continue;
                    }

                    // если первая смена заканчивается на обратном рейсе рейсе
                    if (worksheet.Cells[viezdZezdVGaragePoVihodam[indexOfRowOfTimeEnd] - 1, col].Style.Font.UnderLine)
                    {
                        findSeconSmena = true;
                        continue;
                    }

                    if (worksheet.Cells[middleRow - 1, col].Style.Font
                        .UnderLine) // если первая смена заканчивается на прямом рейсе
                    {
                        findSeconSmena = true;

                        int startAfterFirstSmena = middleRow + 1;

                        for (int row = startAfterFirstSmena;
                             row < viezdZezdVGaragePoVihodam[indexOfRowOfTimeEnd];
                             row++)
                        {
                            Console.WriteLine(ConvertStringToDate(worksheet.Cells[row, col].Text));
                        }

                        int diff = CalculateTimeDifferenceInMinutes(worksheet.Cells[startAfterFirstSmena, col].Text,
                            worksheet.Cells[viezdZezdVGaragePoVihodam[indexOfRowOfTimeEnd] - 1, col].Text);
                        int collNumberInRazbivka = 0;
                        for (int i = 2; i <= 4; i++)
                        {
                            if (diff.ToString() == worksheetRazbivka.Cells[50, i].Text)
                            {
                                collNumberInRazbivka = i;
                                break;
                            }
                        }

                        Console.WriteLine(
                            $"продолжительность обратного рейса = {diff}, смотрим разбивку по {collNumberInRazbivka} колонке");

                        string timeS = ConvertStringToDate(worksheet.Cells[startAfterFirstSmena, col].Text);
                        for (int i = 29; i <= 50; i++)  // заменить индексы на такие, чтобы программа сама считала
                        {
                            DateTime time = DateTime.ParseExact(timeS, "yyyy-MM-ddTHH-mm-ss", null);
                            time = time.AddMinutes(Convert.ToInt32(worksheetRazbivka.Cells[i, collNumberInRazbivka].Text));
                            string tempTime = time.ToString("yyyy-MM-ddTHH-mm-ss");

                            XmlElement newpoint = doc.CreateElement("TTimePoint");
                            newpoint.SetAttribute("routepnt", $"{i-3}");
                            newpoint.SetAttribute("trip", $"{schetchickColonokVAzimut}");
                            newpoint.SetAttribute("time", tempTime);
                            tp.AppendChild(newpoint);
                        }
                    
                        schetchickColonokVAzimut += 1;
                        
                        Console.WriteLine();

                        continue;
                    }

                    // есть и прямой и обратный рейс
                    int firstStart = viezdZezdVGaragePoVihodam[indexOfRowOfTimeStart] + 1;
                    for (int row = firstStart; row < middleRow; row++)
                    {
                        Console.WriteLine(ConvertStringToDate(worksheet.Cells[row, col].Text));
                    }

                    int firstDiff = CalculateTimeDifferenceInMinutes(worksheet.Cells[firstStart, col].Text,
                        worksheet.Cells[middleRow - 1, col].Text);
                    int forwardCollNumberInRazbivka = 0;
                    for (int i = 2; i <= 4; i++)
                    {
                        if (firstDiff.ToString() == worksheetRazbivka.Cells[26, i].Text)
                        {
                            forwardCollNumberInRazbivka = i;
                            break;
                        }
                    }

                    Console.WriteLine(
                        $"продолжительность рейса = {firstDiff}, смотрим разбивку по {forwardCollNumberInRazbivka} колонке");

                    string timeString = ConvertStringToDate(worksheet.Cells[firstStart, col].Text);
                    for (int i = 3; i <= 26; i++)
                    {
                        DateTime time = DateTime.ParseExact(timeString, "yyyy-MM-ddTHH-mm-ss", null);
                        time = time.AddMinutes(Convert.ToInt32(worksheetRazbivka.Cells[i, forwardCollNumberInRazbivka].Text));
                        string tempTime = time.ToString("yyyy-MM-ddTHH-mm-ss");

                        XmlElement newpoint = doc.CreateElement("TTimePoint");
                        // newpoint.SetAttribute("usertripnew", "1");
                        newpoint.SetAttribute("routepnt", $"{i-1}");
                        newpoint.SetAttribute("trip", $"{schetchickColonokVAzimut}");
                        newpoint.SetAttribute("time", tempTime);
                        tp.AppendChild(newpoint);
                    }
                    
                    Console.WriteLine();

                    int secondStart = middleRow + 1;
                    for (int row = secondStart; row < viezdZezdVGaragePoVihodam[indexOfRowOfTimeEnd]; row++)
                    {
                        Console.WriteLine(ConvertStringToDate(worksheet.Cells[row, col].Text));
                    }

                    int secondDiff = CalculateTimeDifferenceInMinutes(worksheet.Cells[secondStart, col].Text,
                        worksheet.Cells[viezdZezdVGaragePoVihodam[indexOfRowOfTimeEnd] - 1, col].Text);
                    int backwardCollNumberInRazbivka = 0;
                    for (int i = 2; i <= 4; i++)
                    {
                        if (secondDiff.ToString() == worksheetRazbivka.Cells[50, i].Text)
                        {
                            backwardCollNumberInRazbivka = i;
                            break;
                        }
                    }

                    Console.WriteLine(
                        $"продолжительность обратного рейса = {secondDiff}, смотрим разбивку по {backwardCollNumberInRazbivka} колонке");

                    
                    timeString = ConvertStringToDate(worksheet.Cells[secondStart, col].Text);
                    for (int i = 29; i <= 50; i++)  // заменить индексы на такие, чтобы программа сама считала
                    {
                        DateTime time = DateTime.ParseExact(timeString, "yyyy-MM-ddTHH-mm-ss", null);
                        time = time.AddMinutes(Convert.ToInt32(worksheetRazbivka.Cells[i, backwardCollNumberInRazbivka].Text));
                        string tempTime = time.ToString("yyyy-MM-ddTHH-mm-ss");

                        XmlElement newpoint = doc.CreateElement("TTimePoint");
                        newpoint.SetAttribute("routepnt", $"{i-3}");
                        newpoint.SetAttribute("trip", $"{schetchickColonokVAzimut}");
                        newpoint.SetAttribute("time", tempTime);
                        tp.AppendChild(newpoint);
                    }
                    
                    schetchickColonokVAzimut += 1;
                    Console.WriteLine();
                }
            }
            
            if (smenaNumber == 2)
            {
                string lasttimestring = ConvertStringToDate(worksheet.Cells[viezdZezdVGaragePoVihodam[indexOfRowOfTimeEnd], parkEndTimeRows[0]].Text);
                XmlElement lasttpoint = doc.CreateElement("TTimePoint");
                lasttpoint.SetAttribute("routepnt", $"{1 + forwardRoutes.Count + backwardRoutes.Count + 1}");
                lasttpoint.SetAttribute("trip", $"{schetchickColonokVAzimut - 1}");
                lasttpoint.SetAttribute("time", lasttimestring);
                tp.AppendChild(lasttpoint);
            }
            // тут нужно закрыть xml документ
            
        }
        
        root.AppendChild(tp);

        doc.AppendChild(root);
    }
}