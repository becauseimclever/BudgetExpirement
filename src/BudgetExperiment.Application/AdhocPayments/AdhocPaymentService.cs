namespace BudgetExperiment.Application.AdhocPayments;

using BudgetExperiment.Domain;

/// <summary>
/// Service for managing adhoc payments.
/// </summary>
public sealed class AdhocPaymentService
{
    private readonly IAdhocPaymentReadRepository _readRepository;
    private readonly IAdhocPaymentWriteRepository _writeRepository;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>
    /// Initializes a new instance of the <see cref="AdhocPaymentService"/> class.
    /// </summary>
    /// <param name="readRepository">The read repository.</param>
    /// <param name="writeRepository">The write repository.</param>
    /// <param name="unitOfWork">The unit of work.</param>
    public AdhocPaymentService(
        IAdhocPaymentReadRepository readRepository,
        IAdhocPaymentWriteRepository writeRepository,
        IUnitOfWork unitOfWork)
    {
        this._readRepository = readRepository ?? throw new ArgumentNullException(nameof(readRepository));
        this._writeRepository = writeRepository ?? throw new ArgumentNullException(nameof(writeRepository));
        this._unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    /// <summary>
    /// Creates a new adhoc payment.
    /// </summary>
    /// <param name="request">The creation request.</param>
    /// <returns>The created adhoc payment response.</returns>
    public async Task<AdhocPaymentResponse> CreateAsync(CreateAdhocPaymentRequest request)
    {
        var money = MoneyValue.Create(request.Currency, request.Amount);
        var adhocPayment = AdhocPayment.Create(request.Description, money, request.Date, request.Category);

        await this._writeRepository.AddAsync(adhocPayment);
        await this._unitOfWork.SaveChangesAsync();

        return ToResponse(adhocPayment);
    }

    /// <summary>
    /// Gets an adhoc payment by ID.
    /// </summary>
    /// <param name="id">The adhoc payment ID.</param>
    /// <returns>The adhoc payment response, or null if not found.</returns>
    public async Task<AdhocPaymentResponse?> GetByIdAsync(Guid id)
    {
        var adhocPayment = await this._readRepository.GetByIdAsync(id);
        return adhocPayment == null ? null : ToResponse(adhocPayment);
    }

    /// <summary>
    /// Gets adhoc payments within a date range.
    /// </summary>
    /// <param name="startDate">Start date (inclusive).</param>
    /// <param name="endDate">End date (inclusive).</param>
    /// <returns>List of adhoc payments in the date range.</returns>
    public async Task<IReadOnlyList<AdhocPaymentResponse>> GetByDateRangeAsync(DateOnly startDate, DateOnly endDate)
    {
        var adhocPayments = await this._readRepository.GetByDateRangeAsync(startDate, endDate);
        return adhocPayments.Select(ToResponse).ToList();
    }

    private static AdhocPaymentResponse ToResponse(AdhocPayment adhocPayment)
    {
        return new AdhocPaymentResponse(
            adhocPayment.Id,
            adhocPayment.Description,
            adhocPayment.Money.Currency,
            adhocPayment.Money.Amount,
            adhocPayment.Date,
            adhocPayment.Category,
            adhocPayment.CreatedUtc,
            adhocPayment.UpdatedUtc);
    }
}
