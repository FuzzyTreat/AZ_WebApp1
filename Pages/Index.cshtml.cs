using AZ_WebApp1.cosmodb;
using AZ_WebApp1.UtilityService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Azure.Cosmos;

namespace AZ_WebApp1.Pages
{
    [BindProperties]
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        ICosmoDBService _cosmoDBService;
        ICipherClient _cipherClient;

        public DataPackage DataPackage { get; set; }
        public string ResponseText { get; set; }

        //public IndexModel(ILogger<IndexModel> logger, ICosmoDBService cosmoDBService)
        //{
        //    _logger = logger;
        //    _cosmoDBService = cosmoDBService;
        //}

        public IndexModel(ILogger<IndexModel> logger, ICipherClient cipherClient)
        {
            _logger = logger;
            _cosmoDBService = null;
            _cipherClient = cipherClient;
            DataPackage = new DataPackage();
            ResponseText = string.Empty;
        }

        public async Task OnGet()
        {
            if (_cosmoDBService != null)
            {
                var traderun = new TradeRun(_cosmoDBService);
                await traderun.FillTradeRun("User1");
            }
        }

        public async Task OnPost()
        {
            if (DataPackage != null && !string.IsNullOrWhiteSpace(DataPackage.Text)) 
            {
                var result = await _cipherClient.ProcessTextAsync(DataPackage);
                ResponseText = result;
                ViewData["ResponseText"] = ResponseText;
            }
        }
    }
}