# Financial Item Dialog Component

## Overview

The `FinancialItemDialog` is a reusable Blazor component designed to handle add/edit operations for various financial entities including recurring schedules and adhoc transactions. This component follows the DRY principle by consolidating common dialog functionality.

## Features

- **Unified Model**: Uses `FinancialItemDialogModel` for all financial entities
- **Flexible Configuration**: Supports different field combinations via parameters
- **Validation**: Built-in client-side validation with error display
- **Responsive**: Works seamlessly across devices
- **FluentUI Integration**: Fully styled with Microsoft FluentUI components
- **Transaction Type Support**: Handles both income and expense transactions

## Usage Examples

### Recurring Schedules (handled by UnifiedScheduleDialog)
Use the `UnifiedScheduleDialog` component for recurring income and expense schedules.

### Adhoc Transactions Management
```razor
<FinancialItemDialog IsVisible="@showDialog"
                     Title="@(isEdit ? "Edit Transaction" : "Add New Transaction")"
                     NameLabel="Description"
                     NamePlaceholder="Enter transaction description..."
                     DateLabel="Transaction Date"
                     SaveButtonText="@(isEdit ? "Update Transaction" : "Add Transaction")"
                     ShowCategoryField="true"
                     ShowRecurrenceField="false"
                     ShowTransactionTypeField="true"
                     Model="@dialogModel"
                     OnCancel="CloseDialog"
                     OnSave="SaveTransaction"
                     OnDelete="DeleteTransaction" />
```

## Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `IsVisible` | `bool` | `false` | Controls dialog visibility |
| `Title` | `string` | `"Edit Item"` | Dialog title |
| `NameLabel` | `string` | `"Name"` | Label for the name/description field |
| `NamePlaceholder` | `string` | `"Enter name..."` | Placeholder for name field |
| `DateLabel` | `string` | `"Date"` | Label for the date field |
| `SaveButtonText` | `string` | `"Save"` | Text for the save button |
| `ShowCategoryField` | `bool` | `false` | Whether to show category field |
| `ShowRecurrenceField` | `bool` | `false` | Whether to show recurrence field |
| `ShowTransactionTypeField` | `bool` | `false` | Whether to show transaction type radio buttons |
| `RecurrenceOptions` | `List<string>` | `["Monthly"]` | Available recurrence options |
| `Model` | `FinancialItemDialogModel` | `new()` | The data model |
| `OnCancel` | `EventCallback` | - | Callback when cancel is clicked |
| `OnSave` | `EventCallback` | - | Callback when save is clicked |
| `OnDelete` | `EventCallback` | - | Callback when delete is clicked |

## FinancialItemDialogModel

### Properties
- `Id` (Guid?) - Entity ID (null for new items)
- `IsEditMode` (bool) - Whether in edit mode
- `Name` (string) - Name/description of the item
- `Currency` (string) - Currency code (default: "USD")
- `Amount` (decimal?) - Monetary amount
- `DateTime` (DateTime) - Date/time value
- `Category` (string?) - Optional category
- `Recurrence` (string) - Recurrence pattern (for schedules)
- `TransactionType` (TransactionType) - Type of transaction (Income or Expense)
- `IsSaving` (bool) - Loading state during save
- `IsDeleting` (bool) - Loading state during delete

### Methods
- `Validate(bool requireName = true)` - Validates the model
- `ClearErrors()` - Clears all validation errors

## Architecture Benefits

1. **DRY Principle**: Eliminates duplicate dialog code across pages
2. **Consistency**: Uniform UI/UX across all financial dialogs
3. **Maintainability**: Single point of change for dialog behavior
4. **Extensibility**: Easy to add new field types or validation rules
5. **Type Safety**: Strongly-typed model with compile-time checking
6. **Unified Approach**: Single dialog handles both income and expense transactions

## File Locations

- **Component**: `src/BudgetExperiment.Client/Components/FinancialItemDialog.razor`
- **Usage Examples**:
  - `src/BudgetExperiment.Client/Pages/FluentCalendar.razor`
  - `src/BudgetExperiment.Client/Components/UnifiedDayDetailsDialog.razor`

## Dependencies

- Microsoft.FluentUI.AspNetCore.Components
- Microsoft.AspNetCore.Components (included in _Imports.razor)
- BudgetExperiment.Domain (for TransactionType enum)

## Related Components

- `UnifiedScheduleDialog` - For managing recurring income/expense schedules
- `UnifiedDayDetailsDialog` - For viewing and editing all items for a specific day
