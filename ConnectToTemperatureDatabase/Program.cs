using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Text;
using System.IO;
using System.Net;
using System.Reflection;
using System.Reflection.Emit;
using Firebase.Auth;
using FireSharp.Interfaces;
using FireSharp.Response;

class Program
{
    static void Main()
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        while (true)
        {
            DateTime currentTime = DateTime.Now;
            if (!(currentTime.Minute % 2 == 0))
            {
                Console.WriteLine("Ожидаю, так как текущее время: " + currentTime.ToLongTimeString());
                System.Threading.Thread.Sleep(60000);
            }
            ExecuteQuery();
            System.Threading.Thread.Sleep(2 * 60000);
        }
    }

    static private async Task a(double temperature)
    {
        string apiKey = "AIzaSyBL00w5zYymNR20QE-mSGsrz9nqKojimQM";
        //FirebaseAuthProvider firebaseAuthProvider = new FirebaseAuthProvider(new FirebaseConfig(apiKey));
        //FirebaseAuthLink link = await firebaseAuthProvider.CreateUserWithEmailAndPasswordAsync("eewrrwe1@g.com", "sdfsfdsf");
        //FirebaseAuthLink link1 = await firebaseAuthProvider.SignInWithEmailAndPasswordAsync("eewrrwe1@g.com", "sdfsfdsf");
        //Console.WriteLine(link1.User.LocalId);

        IFirebaseConfig config = new FireSharp.Config.FirebaseConfig
        {
            AuthSecret = "1qAz883cBDcUSiDG5IcoMTQ4XHO0LHUb2jx9OMyT",
            BasePath = "https://bustemperature-default-rtdb.europe-west1.firebasedatabase.app/"
        };

        IFirebaseClient firebaseClient = new FireSharp.FirebaseClient(config);
        FirebaseResponse response = firebaseClient.Set("Temperature", temperature);
        if (response.StatusCode == System.Net.HttpStatusCode.OK)
        {
            Console.WriteLine("Успешно записано значение: " + temperature + "; запись сделана в: " + DateTime.Now.ToLongTimeString());
        }
        else
        {
            Console.WriteLine("Ошибка при записи данных: " + response.StatusCode);
        }

        //FirebaseResponse response2 = firebaseClient.Get("1UpdateVersion");
        //if (response2.StatusCode == System.Net.HttpStatusCode.OK)
        //{
        //    Console.WriteLine(response.ResultAs<string>());
        //}
        //else
        //{
        //    Console.WriteLine("Ошибка при получении данных: " + response.StatusCode);
        //}

        //FirebaseResponse response1 = firebaseClient.Delete("1UpdateVersion");
        //if (response1.StatusCode == System.Net.HttpStatusCode.OK)
        //{
        //    Console.WriteLine("Успешно удалено!");
        //}
        //else
        //{
        //    Console.WriteLine("Ошибка при удалении данных: " + response.StatusCode);
        //}
    }

    static private async Task<double> ExecuteQuery()
    {

        double result = 0.0;
        string cmdText = "SELECT Value FROM data WHERE DateTime=(SELECT MAX(DateTime) FROM data WHERE Status=0);";

        try
        {

            using (MySqlConnection mySqlConnection = new MySqlConnection("Server=192.168.1.115; Database=datastore; Uid=user1; Pwd=123; CharSet=cp1251;"))
            {
                mySqlConnection.Open();

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
                        //using (StreamWriter streamWriter = new StreamWriter("ex.log", true, Encoding.Default))
                        //{
                        //    streamWriter.WriteLine(DateTime.Now + ": " + "MySQL Exception: " + ex.Message);
                        //}

                    }
                    finally
                    {

                        //using (StreamWriter streamWriter = new StreamWriter("t.txt", false, Encoding.Default))
                        //{
                        //    streamWriter.Write(result.ToString());
                        //}
                        mySqlTransaction.Commit();
                    }
                }
            }

            await a(result);
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

