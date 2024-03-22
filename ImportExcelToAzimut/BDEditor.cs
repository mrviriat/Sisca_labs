using FirebirdSql.Data.FirebirdClient;

namespace ImportExcelToAzimut;

public class BDEditor
{
    
    static string _configurationData = "database=azimut;User=azimut;Password=_azimut#;Role=RL1;Dialect=3;Server=192.168.1.112;Port=3050;Charset=WIN1251";
    
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
}