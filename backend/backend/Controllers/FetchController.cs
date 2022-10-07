using backend.Commands;
using Going.Plaid;
using Going.Plaid.Transactions;
using Going.Plaid.Entity;
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
            var request = new TransactionsGetRequest()
            {
                Options = new TransactionsGetRequestOptions()
                {
                    Count = 100,
                    IncludePersonalFinanceCategory = true
                },
                StartDate = DateOnly.FromDateTime(DateTime.Now - TimeSpan.FromDays(30)),
                EndDate = DateOnly.FromDateTime(DateTime.Now),
                AccessToken = fetchCommand.AccessToken
            };

            var response = await _plaidClient.TransactionsGetAsync(request);
            var transactions = response.Transactions.Select(transaction =>
                transaction.Name + " " +
                transaction.Amount + " " +
                transaction.Date + " " +
                transaction.PersonalFinanceCategory?.Primary + " " +
                transaction.PaymentChannel + " " +
                transaction.TransactionType);

            return Ok(transactions);
        }
    }
}
