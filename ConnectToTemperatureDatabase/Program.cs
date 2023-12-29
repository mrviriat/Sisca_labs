using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Text;



using System.IO;
using System.Net;
using System.Reflection;
using System.Reflection.Emit;





class Program
{
    static void Main()
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        
        //Console.WriteLine("Hello, World!");
        Console.WriteLine(ExecuteQuery());
        Console.ReadLine();
    }

    static private double ExecuteQuery()
    {

        double result = 0.0;
        string cmdText = "SELECT Value FROM data WHERE DateTime=(SELECT MAX(DateTime) FROM data WHERE Status=0);";

        try
        {

            using (MySqlConnection mySqlConnection = new MySqlConnection("Server=192.168.1.115; Database=datastore; Uid=user1; Pwd=123; CharSet=cp1251;"))
            {
                mySqlConnection.Open();

                //if (mySqlConnection.)

                using (MySqlTransaction mySqlTransaction = mySqlConnection.BeginTransaction(IsolationLevel.ReadCommitted))
                {

                    try
                    {
                        MySqlDataReader command = new MySqlCommand(cmdText, mySqlConnection, mySqlTransaction)
                        {
                            CommandType = CommandType.Text,
                            CommandTimeout = 20
                        }.ExecuteReader();
                        command.Read();
                        result = Convert.ToDouble(command["Value"]);
                        command.Close();



                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Внутренняя ошибка: " + ex.Message);
                        using (StreamWriter streamWriter = new StreamWriter("ex.log", true, Encoding.Default))
                        {
                            streamWriter.WriteLine(DateTime.Now + ": " + "MySQL Exception: " + ex.Message);
                        }

                    }
                    finally
                    {

                        using (StreamWriter streamWriter = new StreamWriter("t.txt", false, Encoding.Default))
                        {
                            streamWriter.Write(result.ToString());
                        }
                        mySqlTransaction.Commit();
                    }
                }
            }

        }
        catch (Exception ex2)
        {
            Console.WriteLine("Внешняя ошибка: " + ex2.Message);
            //using (StreamWriter streamWriter2 = new StreamWriter("ex.log", true, Encoding.Default))
            //{
            //    streamWriter2.WriteLine(DateTime.Now + ": " + "MySQL Connect Exception: " + ex2.Message);
            //}
        }

        return result;
    }
}

