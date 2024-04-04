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
                        
                        string routeNameWithoutLastPart = Regex.Replace(routeNames[i + 1], pattern, "");  // Удаляем найденный текст
                        
                        searchRoute = $"{routeNames[i]} ({routeNameWithoutLastPart})";
                    }
                    
                    // Создаем команду для выполнения SQL-запроса с использованием оператора LIKE
                    FbCommand command = new FbCommand("SELECT * FROM WPT WHERE WPT_NAME LIKE @searchRoute", connection, transaction);
                    
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
                            finishRoutes.Add(new WPT_Route(Convert.ToString(reader["WPT_ID"]), Convert.ToString(reader["WPT_NAME"])));
                            
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
                        FbCommand commandd = new FbCommand("SELECT * FROM WPT WHERE WPT_NAME LIKE @searchRoutee", connection, transaction);
                    
                        commandd.Parameters.AddWithValue("@searchRoutee", $"%{searchRoutee}%");
                        
                        using (FbDataReader readerr = commandd.ExecuteReader())
                        {
                            while (readerr.Read())
                            {
                                if (Convert.ToInt32(readerr["WPT_ID"]) == 473)
                                {
                                    continue;
                                }
                                
                                Console.WriteLine("WPT_ID: {0}, WPT_NAME: \"{1}\"", readerr["WPT_ID"], readerr["WPT_NAME"]);  // Выводим значения всех столбцов данной строки
                                finishRoutes.Add(new WPT_Route(Convert.ToString(readerr["WPT_ID"]), Convert.ToString(readerr["WPT_NAME"])));                            }

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
                transaction.Commit();  // Завершаем транзакцию
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
                        routesIndexes.Add(reader.Depth+1);
                        Console.WriteLine($"{reader.Depth+1}: {reader.GetString(1)}");
                    }
                }
            }
        }

        return routesIndexes;
    }

    private string ConvertStringToDate(string timeString )
    {
        string[] timeParts = timeString.Split(':');  // Разделение строки на часы и минуты
        
        int hours = int.Parse(timeParts[0]);  // Получение часов и минут из строки
        int minutes = int.Parse(timeParts[1]);

        string time;
        
        if (hours < 4)  // Создание объекта DateTime с датой 30/31 декабря 1899 года и указанием времени
        {
            time = $"1899-12-31T{hours}-{minutes}-00";
        }
        else
        {
            time = $"1899-12-30T{hours}-{minutes}-00";
        }
        
        return time;
    }
    
    public void ReadExcelForAllTimes(string filePath)
    {
        List<int> routesIndexes = ReadExcelForRotes(filePath);
        List<int> parkEndTimeRows = new List<int>();
        
        int vihod = 1;
        
        using (ExcelPackage package = new ExcelPackage(new FileInfo(filePath)))  // Открываем файл Excel с помощью EPPlus
        {
            ExcelWorksheet worksheet = package.Workbook.Worksheets[0]; // Получаем первый лист в книге

            int row = vihod * 2 - 1;
            
            for (int i = 1; i < 50; i++)
            {
                if (worksheet.Cells[routesIndexes[row], i].Text.Length > 0 &&
                    worksheet.Cells[routesIndexes[row], i].Text.Length <= 5)
                {
                    parkEndTimeRows.Add(i);
                    // Console.WriteLine(worksheet.Cells[routesIndexes[row], i].Text);
                    // Console.WriteLine($"Строка - {routesIndexes[row]}");
                    // Console.WriteLine($"Колонка - {i}");
                }
            }
        }
        
        using (ExcelPackage package = new ExcelPackage(new FileInfo(filePath)))  // Открываем файл Excel с помощью EPPlus
        {
            ExcelWorksheet worksheet = package.Workbook.Worksheets[0]; // Получаем первый лист в книге

            int indexOfRowOfTimeStart = vihod * 2 - 2;
            int indexOfRowOfTimeEnd = vihod * 2 - 1;
            
            int middleRow = (routesIndexes[indexOfRowOfTimeEnd] - routesIndexes[indexOfRowOfTimeStart]) / 2 +
                            routesIndexes[indexOfRowOfTimeStart];
            
            for (int col = 3; col <= parkEndTimeRows[0]; col++)
            {
                if (worksheet.Cells[routesIndexes[indexOfRowOfTimeStart] + 1, col].Text == "обед")
                {
                    continue;
                }
                
                for (int row = routesIndexes[indexOfRowOfTimeStart] + 1; row < middleRow; row++)
                {
                    Console.WriteLine(ConvertStringToDate(worksheet.Cells[row, col].Text));

                    if (worksheet.Cells[row, col].Style.Font.UnderLine)
                    {
                        Console.WriteLine("Конец первой смены");
                    }
                }

                Console.WriteLine();
                
                for (int row = middleRow + 1; row < routesIndexes[indexOfRowOfTimeEnd]; row++)
                {
                    Console.WriteLine(ConvertStringToDate(worksheet.Cells[row, col].Text));
                    
                    if (worksheet.Cells[row, col].Style.Font.UnderLine)
                    {
                        Console.WriteLine("Конец первой смены");
                    }
                }
                
                Console.WriteLine();
            }
        }
    }

}