using AZ_WebApp1.cosmodb;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Azure.Cosmos;

namespace AZ_WebApp1.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        ICosmoDBService _cosmoDBService;

        public IndexModel(ILogger<IndexModel> logger, ICosmoDBService cosmoDBService)
        {
            _logger = logger;
            _cosmoDBService = cosmoDBService;
        }

        public async Task OnGet()
        {
            var traderun = new TradeRun(_cosmoDBService);
            await traderun.FillTradeRun("User1");
        }
    }
}