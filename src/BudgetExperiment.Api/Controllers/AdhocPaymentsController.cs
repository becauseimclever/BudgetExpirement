namespace BudgetExperiment.Api.Controllers;

using BudgetExperiment.Application.AdhocPayments;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Controller for managing adhoc payments.
/// </summary>
[ApiController]
[Route("api/v1/adhocpayments")]
[Produces("application/json")]
public sealed class AdhocPaymentsController : ControllerBase
{
    private readonly AdhocPaymentService _service;

    /// <summary>
    /// Initializes a new instance of the <see cref="AdhocPaymentsController"/> class.
    /// </summary>
    /// <param name="service">The adhoc payment service.</param>
    public AdhocPaymentsController(AdhocPaymentService service)
    {
        this._service = service ?? throw new ArgumentNullException(nameof(service));
    }

    /// <summary>
    /// Creates a new adhoc payment.
    /// </summary>
    /// <param name="request">The adhoc payment creation request.</param>
    /// <returns>The created adhoc payment.</returns>
    [HttpPost]
    [ProducesResponseType<AdhocPaymentResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AdhocPaymentResponse>> CreateAsync([FromBody] CreateAdhocPaymentRequest request)
    {
        var response = await this._service.CreateAsync(request);
        return this.CreatedAtRoute("GetAdhocPaymentById", new { id = response.Id }, response);
    }

    /// <summary>
    /// Gets an adhoc payment by ID.
    /// </summary>
    /// <param name="id">The adhoc payment ID.</param>
    /// <returns>The adhoc payment.</returns>
    [HttpGet("{id}", Name = "GetAdhocPaymentById")]
    [ProducesResponseType<AdhocPaymentResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AdhocPaymentResponse>> GetByIdAsync([FromRoute] Guid id)
    {
        var response = await this._service.GetByIdAsync(id);
        if (response == null)
        {
            return this.NotFound();
        }

        return this.Ok(response);
    }

    /// <summary>
    /// Gets adhoc payments within a date range.
    /// </summary>
    /// <param name="startDate">Start date (inclusive).</param>
    /// <param name="endDate">End date (inclusive).</param>
    /// <returns>List of adhoc payments in the date range.</returns>
    [HttpGet("by-date-range")]
    [ProducesResponseType<IReadOnlyList<AdhocPaymentResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IReadOnlyList<AdhocPaymentResponse>>> GetByDateRangeAsync(
        [FromQuery] DateOnly startDate,
        [FromQuery] DateOnly endDate)
    {
        if (startDate > endDate)
        {
            return this.BadRequest("startDate must be less than or equal to endDate");
        }

        var adhocPayments = await this._service.GetByDateRangeAsync(startDate, endDate);
        return this.Ok(adhocPayments);
    }

    /// <summary>
    /// Deletes an adhoc payment.
    /// </summary>
    /// <param name="id">The adhoc payment ID.</param>
    /// <returns>No content if successful.</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAsync([FromRoute] Guid id)
    {
        var deleted = await this._service.DeleteAsync(id);
        return deleted ? this.NoContent() : this.NotFound();
    }
}
