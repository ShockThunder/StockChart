using System.Globalization;

using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;

using JetBrains.Annotations;

using MediatR;

using MongoDB.Driver;

namespace StockApi.Query;

[PublicAPI]
public class GetStockDataHandler : IRequestHandler<GetStockDataQuery, List<StockEntry>>
{
    private IMongoCollection<StockEntry> _stockData;
    private readonly IConfiguration _configuration;
    private readonly string[] _scopes = {SheetsService.Scope.Spreadsheets};
    private const string APPLICATION_NAME = "StockApp";
    private const string SPREADSHEET_ID = "1LuGgCKXU2GIH-6vUGsg6g62URC_muakIRuWiEr3LE3E";
    private const string SHEET_NAME = "Sheet1";
    private const string RANGE = $"{SHEET_NAME}!A2:B252";

    private SheetsService _service;

    public GetStockDataHandler(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<List<StockEntry>> Handle(GetStockDataQuery request, CancellationToken cancellationToken)
    {
        var mongoClient = new MongoClient(
            _configuration["DatabaseSettings:ConnectionString"]);
        var mongoDatabase = mongoClient.GetDatabase(
            _configuration.GetSection("DatabaseSettings").GetValue<string>("DatabaseName"));

        _stockData = mongoDatabase.GetCollection<StockEntry>(
            _configuration.GetSection("DatabaseSettings").GetValue<string>("CollectionName"));
        var result = await (await _stockData.FindAsync(_ => true)).ToListAsync();

        if (result.Count == 0)
        {
            try
            {
                result = GetFromSheets();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.InnerException);
                return result;
            }


            await _stockData.InsertManyAsync(result);
        }

        return result;
    }

    private List<StockEntry> GetFromSheets()
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

        var values = request.Execute().Values;

        if (values != null && values.Count > 0)
            return values.Select(value => new StockEntry
            {
                DateTime = Convert.ToDateTime(value[0], CultureInfo.InvariantCulture).ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss.fffffffK"),
                Value = Convert.ToDouble(value[1], CultureInfo.InvariantCulture)
            }).ToList();

        return new List<StockEntry>();
    }
}