using FirebirdSql.Data.FirebirdClient;
using System;
using ExcelDataReader;
using System.Data;
using System.IO;

namespace ImportExcelToAzimut;

public class BDEditor
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
                        @"C:\Users\a.gavrilenko\Desktop\FileWith55Data.xml");
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

    public void GetPointsNames(List<string> routeNames)
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

                foreach (string searchRoute in routeNames)
                {
                    // Создаем команду для выполнения SQL-запроса с использованием оператора LIKE
                    FbCommand command = new FbCommand("SELECT * FROM WPT WHERE WPT_NAME LIKE @searchRoute", connection, transaction);
                    
                    command.Parameters.AddWithValue("@searchRoute", $"%{searchRoute}%");

                    using (FbDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Console.WriteLine("WPT_ID: {0}, WPT_NAME: \"{1}\"", reader["WPT_ID"], reader["WPT_NAME"]);  // Выводим значения всех столбцов данной строки
                        }

                        Console.WriteLine("===============================");
                    }
                }
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
                            string route = reader.GetString(0).TrimEnd() + " (";
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
                        string route = reader.GetString(0).TrimEnd() + " (";
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
            // using (var reader = ExcelReaderFactory.CreateReader(stream))
            // {
            //     while (reader.Read()) // Читаем построчно
            //     {
            //         if (reader.GetString(1) == "АП г. Гродно")
            //         {
            //             routesIndexes.Add(reader.Depth+1);
            //             Console.WriteLine($"{reader.Depth+1}: {reader.GetString(1)}");
            //         }
            //     }
            // }
            
            using (var reader = ExcelReaderFactory.CreateReader(stream))
            {
                reader.Read(); 
                reader.Read(); 
                reader.Read(); 
                reader.Read();
                reader.Read(); 
                
                var valueC5 = reader.GetValue(2); // 0 - индекс колонки A
                
                Console.WriteLine("Значение из ячейки С5: " + valueC5.ToString());
                
                reader.Reset();
                
                reader.Read(); 
                reader.Read(); 
                reader.Read(); 
                reader.Read();
                reader.Read(); 
                reader.Read(); 
                reader.Read();
                reader.Read();
                
                var valueC8 = reader.GetValue(2); // 0 - индекс колонки A
                
                Console.WriteLine("Значение из ячейки С8: " + valueC8.ToString());
                
                reader.Reset();
            }
            
        }

        return routesIndexes;
    }

}