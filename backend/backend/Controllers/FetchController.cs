using backend.Commands;
using backend.Models;
using Going.Plaid;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers {
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class FetchController : ControllerBase {
        private readonly PlaidClient _plaidClient;

        public FetchController(PlaidClient plaidClient) {
            _plaidClient = plaidClient;
        }

        [HttpGet]
        public async Task<IActionResult> Transactions([FromQuery] FetchCommand fetchCommand) {
            var request = new Going.Plaid.Transactions.TransactionsGetRequest()
            {
                Options = new Going.Plaid.Entity.TransactionsGetRequestOptions()
                {
                    Count = 100
                },
                StartDate = DateOnly.FromDateTime(DateTime.Now - TimeSpan.FromDays(30)),
                EndDate = DateOnly.FromDateTime(DateTime.Now),
                AccessToken = fetchCommand.AccessToken
            };

            var response = await _plaidClient.TransactionsGetAsync(request);
            var list = new List<string>();
            
            foreach (var transaction in response.Transactions) {
                list.Add(
                    transaction.Name + " " +
                    transaction.Amount + " " +
                    transaction.Date + " " +
                    string.Join(':', transaction.Category ?? Enumerable.Empty<string>()) + " " +
                    transaction.PaymentChannel + " " +
                    transaction.TransactionType
                );
            }

            return Ok(list);
        }
    }
}
