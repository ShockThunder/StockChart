using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;

using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<StockService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
    {
        var forecast = Enumerable.Range(1, 5).Select(index =>
                new WeatherForecast
                (
                    DateTime.Now.AddDays(index),
                    Random.Shared.Next(-20, 55),
                    summaries[Random.Shared.Next(summaries.Length)]
                ))
            .ToArray();
        return forecast;
    })
    .WithName("GetWeatherForecast");

app.MapGet("/stockdata", ([FromServices] StockService service) =>
{
    service.GetData();
});

app.Run();

internal record WeatherForecast(DateTime Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

public class StockService
{
    public string[] Scopes = {SheetsService.Scope.Spreadsheets};
    public string ApplicationName = "StockApp";
    public string SpreadsheetId = "1LuGgCKXU2GIH-6vUGsg6g62URC_muakIRuWiEr3LE3E";
    public string SheetName = "Sheet1";
    private SheetsService Service;

    public void GetData()
    {
        GoogleCredential credential;
        using (var stream = new FileStream("molten-snowfall-229304-c60799baa9b3.json", FileMode.Open, FileAccess.Read))
        {
            credential = GoogleCredential.FromStream(stream).CreateScoped(Scopes);
        }

        Service = new SheetsService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential,
            ApplicationName = ApplicationName
        });
        
        ReadEntries();
    }

    private void ReadEntries()
    {
        var range = $"{SheetName}!A1:B50";
        var request = Service.Spreadsheets.Values.Get(SpreadsheetId, range);

        var response = request.Execute();
        var values = response.Values;

        if (values != null && values.Count > 0)
        {
            foreach (var value in values)
            {
                Console.WriteLine($"{value[0]} ----- {value[1]}");
            }
        }
        else
        {
            Console.WriteLine("NO DATA");
        }
    }
}