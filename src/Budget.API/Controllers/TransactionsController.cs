using Asp.Versioning;
using Budget.Abstractions.Services;
using Budget.Models;
using Microsoft.AspNetCore.Mvc;

namespace Budget.API.Controllers
{
    /// <summary>
    /// Controller for managing transactions.
    /// </summary>
    [ApiController]
    [Route("api/transactions")]
    [ApiVersion("1.0")] // Specify version 1.0
    public class TransactionsController : ControllerBase
    {
        private readonly ITransactionService _transactionService;

        /// <summary>
        /// Initializes a new instance of the <see cref="TransactionsController"/> class.
        /// </summary>
        /// <param name="transactionService">The transaction service for managing transactions.</param>
        public TransactionsController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        /// <summary>
        /// Retrieves a transaction by its ID.
        /// </summary>
        /// <param name="id">The ID of the transaction.</param>
        /// <returns>The transaction if found; otherwise, a 404 status code.</returns>
        [HttpGet("{id}")]
        [EndpointName("GetTransactionById")]
        [EndpointSummary("Retrieves a transaction by its ID.")]
        [EndpointDescription("This endpoint fetches the details of a transaction using its unique identifier. If the transaction is not found, a 404 status code is returned.")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Transaction), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetTransactionById(Guid id)
        {
            var transaction = _transactionService.GetTransactionById(id);
            if (transaction == null)
            {
                return NotFound();
            }

            return Ok(transaction);
        }

        /// <summary>
        /// Retrieves all transactions for a specific account.
        /// </summary>
        /// <param name="accountId">The ID of the account.</param>
        /// <returns>A list of transactions for the specified account.</returns>
        [HttpGet("by-account/{accountId}")]
        [EndpointName("GetTransactionsByAccount")]
        [EndpointSummary("Retrieves all transactions for a specific account.")]
        [EndpointDescription("This endpoint fetches all transactions associated with a specific account using the account's unique identifier.")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(IEnumerable<Transaction>), StatusCodes.Status200OK)]
        public IActionResult GetTransactionsByAccount(Guid accountId)
        {
            var transactions = _transactionService.GetTransactionsByAccount(accountId);
            return Ok(transactions);
        }

        /// <summary>
        /// Retrieves all transactions with pagination.
        /// </summary>
        /// <param name="pageNumber">The page number (1-based).</param>
        /// <param name="pageSize">The number of items per page.</param>
        /// <returns>A paginated collection of transactions.</returns>
        [HttpGet]
        [EndpointName("GetAllTransactions")]
        [EndpointSummary("Retrieves all transactions with pagination.")]
        [EndpointDescription("This endpoint fetches all transactions with pagination support. The page number and page size can be specified as query parameters.")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(IEnumerable<Transaction>), StatusCodes.Status200OK)]
        public IActionResult GetAllTransactions([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var transactions = _transactionService.GetAllTransactions(pageNumber, pageSize);
            return Ok(transactions);
        }
    }
}