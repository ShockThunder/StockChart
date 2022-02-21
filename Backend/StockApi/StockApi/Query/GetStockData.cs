using MediatR;

namespace StockApi.Query;

public class GetStockDataQuery: IRequest<List<StockEntry>>
{
    public List<StockEntry> StockEntries { get; set; }
}