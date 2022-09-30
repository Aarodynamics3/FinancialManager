using backend.Commands;
using backend.Models;
using Going.Plaid;
using Going.Plaid.Entity;
using Going.Plaid.Item;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LinkController : ControllerBase {
        private readonly PlaidClient _plaidClient;
        private readonly PlaidCredentials _plaidCredentials;

        public LinkController(PlaidClient plaidClient, PlaidCredentials plaidCredentials) {
            _plaidClient = plaidClient;
            _plaidCredentials = plaidCredentials;
        }

        [HttpGet]
        // Creates the temporary link token that is used to initialize the link.
        public async Task<IActionResult> CreateLinkToken() {
            try {
                // Create the temporary link token.
                var response = await _plaidClient.LinkTokenCreateAsync(new Going.Plaid.Link.LinkTokenCreateRequest()
                {
                    ClientName = "Client Name",
                    CountryCodes = new List<CountryCode>() { CountryCode.Us },
                    Products = new List<Products> { Products.Auth, Products.Transactions },
                    User = new LinkTokenCreateRequestUser
                    {
                        ClientUserId = Guid.NewGuid().ToString()
                    }
                });

                // If there is an error, return a 400 bad request.
                if (response.Error is not null)
                    return Error(response.Error);

                // Return the link token.
                return Ok(response.LinkToken);
            } catch (Exception e) {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }

        [HttpPost]
        // Exchanges the public token for the permanent access token. 
        public async Task<IActionResult> ExchangePublicToken(ExchangePublicTokenCommand command) {
            var request = new ItemPublicTokenExchangeRequest() {
                PublicToken = command.Token
            };

            // Calls the Plaid API to exchange the token.
            var response = await _plaidClient.ItemPublicTokenExchangeAsync(request);

            if (response.Error is not null)
                return Error(response.Error);

            //// TODO Remove.
            //_plaidClient.AccessToken = response.AccessToken;
            // Set the fields in plaid credentials so the frontend can use them to make requests.
            _plaidCredentials.AccessToken = response.AccessToken;
            _plaidCredentials.ItemId = response.ItemId;

            // Return the credentials.
            return Ok(_plaidCredentials);
        }

        ObjectResult Error(Going.Plaid.Errors.PlaidError error) {
            return StatusCode(StatusCodes.Status400BadRequest, error);
        }
    }
}
