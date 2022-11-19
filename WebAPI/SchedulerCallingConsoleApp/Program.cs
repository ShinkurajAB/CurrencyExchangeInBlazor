using SchedulerCallingConsoleApp;
using System.Net.Http.Headers;
using System.Text.Json;

HttpClient client = new HttpClient();
client.Timeout = TimeSpan.FromHours(1);
string logsDirectory = Path.Combine(Environment.CurrentDirectory, "BaseFiles");
string FIlePath = logsDirectory + "\\BaseFile.json";
BaseFileHandle obj = new BaseFileHandle();
string json = File.ReadAllText(FIlePath);
obj = JsonSerializer.Deserialize<BaseFileHandle>(json);

Console.WriteLine(obj.BaseURL);

client.BaseAddress = new Uri(obj.BaseURL);
client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));



// Calling Import Currency
var result = client.GetAsync("api/Scheduler/ImportCurrency");
string message = await result.Result.Content.ReadAsStringAsync();
if (result.Result.StatusCode == System.Net.HttpStatusCode.OK)
{

    Console.WriteLine(message);
}
else
{
    Console.WriteLine("Import Currency Scheduler Get an Error Please check API Logs");
    Console.WriteLine("Error In response :" + message);
    Console.ReadKey();
}


// Calling Import Exchange Rate

result= client.GetAsync("api/Scheduler/ImportCurrencyExchangeRate");
message = await result.Result.Content.ReadAsStringAsync();

if (result.Result.StatusCode == System.Net.HttpStatusCode.OK)
{

    Console.WriteLine(message);
   
}
else
{
    Console.WriteLine("Import Exchange Rate Scheduler Get an Error Please check API Logs");
    Console.WriteLine("Error In response :" + message);
    Console.ReadKey();
}