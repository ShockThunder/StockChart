using System.Globalization;

using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;

namespace StockApi;

public class StockService
{
    private readonly string[] _scopes = {SheetsService.Scope.Spreadsheets};
    private const string APPLICATION_NAME = "StockApp";
    private const string SPREADSHEET_ID = "1LuGgCKXU2GIH-6vUGsg6g62URC_muakIRuWiEr3LE3E";
    private const string SHEET_NAME = "Sheet1";
    private const string RANGE = $"{SHEET_NAME}!A2:B50";

    private SheetsService _service;

    public List<StockEntry> GetData()
    {
        GoogleCredential credential;
        using (var stream = new FileStream("molten-snowfall-229304-c60799baa9b3.json", FileMode.Open, FileAccess.Read))
        {
            credential = GoogleCredential.FromStream(stream).CreateScoped(_scopes);
        }

        _service = new SheetsService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential,
            ApplicationName = APPLICATION_NAME
        });
        
        return ReadEntries();
    }

    private List<StockEntry> ReadEntries()
    {
        var request = _service.Spreadsheets.Values.Get(SPREADSHEET_ID, RANGE);

        var response = request.Execute();
        var values = response.Values;

        if (values != null && values.Count > 0)
            return values.Select(value => new StockEntry
            {
                DateTime = Convert.ToDateTime(value[0], CultureInfo.InvariantCulture),
                Value = Convert.ToDouble(value[1], CultureInfo.InvariantCulture)
            }).ToList();

        return new List<StockEntry>();
    }
}