using Asp.Versioning;
using Budget.Abstractions.Services;
using Budget.Models;
using Microsoft.AspNetCore.Mvc;

namespace Budget.API.Controllers
{
	/// <summary>
	/// Controller for managing accounts.
	/// </summary>
	[ApiController]
	[Route("api/accounts")]
	[ApiVersion("1.0")] // Specify version 1.0
	public class AccountsController : ControllerBase
	{
		private readonly IAccountService accountService;

		/// <summary>
		/// Initializes a new instance of the <see cref="AccountsController"/> class.
		/// </summary>
		/// <param name="accountService">The account service for budget operations.</param>
		public AccountsController(IAccountService accountService)
		{
			this.accountService = accountService;
		}

		/// <summary>
		/// Creates a new account.
		/// </summary>
		/// <param name="account">The account to create.</param>
		/// <returns>The created account.</returns>
		[HttpPost]
		[EndpointName("CreateAccount")]
		[EndpointSummary("Creates a new account and returns the created account.")]
		[EndpointDescription("This endpoint allows the creation of a new account. The account details must be provided in the request body.")]
		[Produces("application/json")]
		[ProducesResponseType(typeof(Account), StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public IActionResult CreateAccount([FromBody] Account account)
		{
			accountService.CreateAccount(account);
			return CreatedAtAction(nameof(GetAccountById), new { id = account.Id }, account);
		}

		/// <summary>
		/// Retrieves an account by its ID.
		/// </summary>
		/// <param name="id">The ID of the account.</param>
		/// <returns>The account if found; otherwise, a 404 status code.</returns>
		[HttpGet("{id}")]
		[EndpointName("GetAccountById")]
		[EndpointSummary("Retrieves an account by its ID.")]
		[EndpointDescription("This endpoint fetches the details of an account using its unique identifier. If the account is not found, a 404 status code is returned.")]
		[Produces("application/json")]
		[ProducesResponseType(typeof(Account), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public IActionResult GetAccountById([FromRoute] Guid id)
		{
			try
			{
				var account = accountService.GetAccountById(id);
				return Ok(account);
			}
			catch (KeyNotFoundException)
			{
				return NotFound();
			}
		}

		/// <summary>
		/// Retrieves all accounts.
		/// </summary>
		/// <returns>A collection of all accounts.</returns>
		[HttpGet]
		[EndpointName("GetAllAccounts")]
		[EndpointSummary("Retrieves all accounts.")]
		[EndpointDescription("This endpoint fetches all accounts available in the system.")]
		[Produces("application/json")]
		[ProducesResponseType(typeof(IEnumerable<Account>), StatusCodes.Status200OK)]
		public IActionResult GetAllAccounts()
		{
			var accounts = accountService.GetAllAccounts();
			return Ok(accounts);
		}

		/// <summary>
		/// Updates an existing account.
		/// </summary>
		/// <param name="account">The account to update.</param>
		/// <returns>No content if the update is successful.</returns>
		[HttpPut]
		[EndpointName("UpdateAccount")]
		[EndpointSummary("Updates an existing account.")]
		[EndpointDescription("This endpoint updates the details of an existing account. The account ID must be provided in the request body.")]
		[Produces("application/json")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public IActionResult UpdateAccount([FromBody] Account account)
		{
			try
			{
				accountService.UpdateAccount(account);
				return NoContent();
			}
			catch (KeyNotFoundException)
			{
				return NotFound();
			}
		}

		/// <summary>
		/// Deletes an account by its ID.
		/// </summary>
		/// <param name="id">The ID of the account to delete.</param>
		/// <returns>No content if the deletion is successful.</returns>
		[HttpDelete("{id}")]
		[EndpointName("DeleteAccount")]
		[EndpointSummary("Deletes an account by its ID.")]
		[EndpointDescription("This endpoint deletes an account using its unique identifier.")]
		[Produces("application/json")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public IActionResult DeleteAccount([FromRoute] Guid id)
		{
			try
			{
				accountService.DeleteAccount(id);
				return NoContent();
			}
			catch (KeyNotFoundException)
			{
				return NotFound();
			}
		}

		[HttpPost("{accountId}/transactions")]
		[EndpointName("AddTransactionToAccount")]
		[EndpointSummary("Adds a transaction to a specific account.")]
		[ProducesResponseType(typeof(Transaction), StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public IActionResult AddTransactionToAccount([FromRoute] Guid accountId, [FromBody] Transaction transaction)
		{
			try
			{
				accountService.AddTransaction(accountId, transaction);
				return CreatedAtAction(nameof(GetAccountById), new { id = accountId }, transaction);
			}
			catch (KeyNotFoundException)
			{
				return NotFound();
			}
		}

		[HttpPut("transactions/{transactionId}")]
		[EndpointName("UpdateTransaction")]
		[EndpointSummary("Updates a transaction associated with an account.")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public IActionResult UpdateTransaction([FromRoute] Guid transactionId, [FromBody] Transaction transaction)
		{
			try
			{
				transaction.Id = transactionId;
				accountService.UpdateTransaction(transaction);
				return NoContent();
			}
			catch (KeyNotFoundException)
			{
				return NotFound();
			}
		}

		[HttpDelete("transactions/{transactionId}")]
		[EndpointName("DeleteTransaction")]
		[EndpointSummary("Deletes a transaction by its ID.")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public IActionResult DeleteTransaction([FromRoute] Guid transactionId)
		{
			try
			{
				accountService.DeleteTransaction(transactionId);
				return NoContent();
			}
			catch (KeyNotFoundException)
			{
				return NotFound();
			}
		}

		[HttpPost("{accountId}/transactions/bulk")]
		[EndpointName("AddTransactionsInBulk")]
		[EndpointSummary("Adds multiple transactions to a specific account in bulk.")]
		[ProducesResponseType(StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public IActionResult AddTransactionsInBulk([FromRoute] Guid accountId, IFormFile csvFile)
		{
			if (csvFile == null || csvFile.Length == 0)
			{
				return BadRequest("CSV file is required.");
			}

			try
			{
				using var reader = new StreamReader(csvFile.OpenReadStream());
				var transactions = new List<Transaction>();
				string? line;
				while ((line = reader.ReadLine()) != null)
				{
					var values = line.Split(',');
					if (values.Length != 3)
					{
						return BadRequest("Invalid CSV format. Each line must contain description, amount, and date.");
					}

					transactions.Add(new Transaction
					{
						Description = values[0],
						Amount = decimal.Parse(values[1]),
						TransactionDate = DateTime.Parse(values[2])
					});
				}

				accountService.AddTransactionsInBulk(accountId, transactions);
				return CreatedAtAction(nameof(GetAccountById), new { id = accountId }, transactions);
			}
			catch (FormatException)
			{
				return BadRequest("Invalid data format in CSV file.");
			}
			catch (KeyNotFoundException)
			{
				return NotFound("Account not found.");
			}
		}
	}
}