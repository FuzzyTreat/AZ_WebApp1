using Microsoft.Azure.Cosmos;
using System.ComponentModel.DataAnnotations;

namespace AZ_WebApp1.cosmodb
{
    public class TradeRun
    {
        private ICosmoDBService _cosmoDBService;

        public string? id { get; set; }
        public string? User { get; set; }
        public string? SourceSystem { get; set; }
        public string? DestinationSystem { get; set; }
        public string? FromStation { get; set; }
        public string? ToStation { get; set; }
        public DateTime TradeDate { get; set; }
        public double TotalSoldValue { get; set; }
        public double TotalBoughtValue { get; set; }
        public int TotalSoldScu { get; set; }
        public int TotalBoughtScu { get; set; }

        public TradeItem[]? Cargo { get; set; }

        public TradeRun(ICosmoDBService cosmoDBService)
        {
            _cosmoDBService = cosmoDBService;
        }

        public void UpdateSummary()
        {
            if (Cargo != null)
            {
                TotalSoldScu = Cargo.Sum(r => r.ScuSold);
                TotalSoldValue = Cargo.Sum(r => r.ScuSold * r.SalesPrice);

                TotalBoughtValue = Cargo.Sum(r => r.ScuBought * r.PurchasePrice);
                TotalBoughtScu = Cargo.Sum(r => r.ScuBought);
            }
        }

        public async Task FillTradeRun(string userid)
        {
            var parameterizedQuery = new QueryDefinition(
                query: $"SELECT * FROM TradeRuns t WHERE t.User = @userid"
            ).WithParameter("@userid", userid);

            var res = await _cosmoDBService.ExecuteParameterizedQuery<TradeRun>(parameterizedQuery);

            if (res != null && res.Count >= 1)
            {
                var tradeRun = res[0];

                id = tradeRun.id;
                User = tradeRun.User;
                SourceSystem = tradeRun.SourceSystem;
                DestinationSystem = tradeRun.DestinationSystem;
                FromStation = tradeRun.FromStation;
                ToStation = tradeRun.ToStation;
                TradeDate = tradeRun.TradeDate;
                TotalSoldValue = tradeRun.TotalSoldValue;
                TotalBoughtValue = tradeRun.TotalBoughtValue;
                TotalSoldScu = tradeRun.TotalBoughtScu;
                TotalBoughtScu = tradeRun.TotalBoughtScu;

                Cargo = tradeRun.Cargo;
            }
        }

        public async Task UpdateTradeRun()
        {

        }
        public async Task DeleteTradeRun()
        {

        }
        public async Task InsertTradeRun()
        {

        }
    }

    public class TradeItem
    {
        public string? CargoName { get; set; }
        public int ScuBought { get; set; }
        public int ScuSold { get; set; }
        public double PurchasePrice { get; set; }
        public double SalesPrice { get; set; }
        public bool IsSold { get; set; }
    }
}
