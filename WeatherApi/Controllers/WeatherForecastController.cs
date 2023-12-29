using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System.Data;
using System.Text;

namespace WeatherApi.Controllers
{
    //[ApiController]
    //[Route("[controller]")]
    //public class WeatherForecastController : ControllerBase
    //{
    //    private static readonly string[] Summaries = new[]
    //    {
    //        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    //    };

    //    private readonly ILogger<WeatherForecastController> _logger;

    //    public WeatherForecastController(ILogger<WeatherForecastController> logger)
    //    {
    //        _logger = logger;
    //    }

    //    [HttpGet(Name = "GetWeatherForecast")]
    //    public IEnumerable<WeatherForecast> Get()
    //    {
    //        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
    //        {
    //            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
    //            TemperatureC = Random.Shared.Next(-20, 55),
    //            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
    //        })
    //        .ToArray();
    //    }
    //}

    

    [ApiController]
    [Route("[controller]")]
    public class HelloWorldController : ControllerBase
    {

        [HttpGet]
        public IActionResult Get()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            return Ok(new { Temperature = ExecuteQuery() });
        }

        private double ExecuteQuery()
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
                            //Console.WriteLine("¬нутренн€€ ошибка: " + ex.Message);
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

            }
            catch (Exception ex2)
            {
                //Console.WriteLine("¬нешн€€ ошибка: " + ex2.Message);
                //using (StreamWriter streamWriter2 = new StreamWriter("ex.log", true, Encoding.Default))
                //{
                //    streamWriter2.WriteLine(DateTime.Now + ": " + "MySQL Connect Exception: " + ex2.Message);
                //}
            }

            return result;
        }
    }

}
