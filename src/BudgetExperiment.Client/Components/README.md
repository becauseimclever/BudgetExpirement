# Financial Item Dialog Component

## Overview

The `FinancialItemDialog` is a reusable Blazor component designed to handle add/edit operations for various financial entities including bills, expenses, and adhoc payments. This component follows the DRY principle by consolidating common dialog functionality.

## Features

- **Unified Model**: Uses `FinancialItemDialogModel` for all financial entities
- **Flexible Configuration**: Supports different field combinations via parameters
- **Validation**: Built-in client-side validation with error display
- **Responsive**: Works seamlessly across devices
- **FluentUI Integration**: Fully styled with Microsoft FluentUI components

## Usage Examples

### Bills Management
```razor
<FinancialItemDialog IsVisible="@showDialog"
                     Title="@(isEdit ? "Edit Bill" : "Add New Bill")"
                     NameLabel="Bill Name"
                     NamePlaceholder="Enter bill name..."
                     DateLabel="Due Date"
                     SaveButtonText="@(isEdit ? "Update Bill" : "Add Bill")"
                     ShowCategoryField="false"
                     ShowRecurrenceField="true"
                     RecurrenceOptions="@billRecurrenceOptions"
                     Model="@dialogModel"
                     OnCancel="CloseDialog"
                     OnSave="SaveBill" />
```

### Expenses Management
```razor
<FinancialItemDialog IsVisible="@showDialog"
                     Title="@(isEdit ? "Edit Expense" : "Add New Expense")"
                     NameLabel="Description"
                     NamePlaceholder="Enter expense description..."
                     DateLabel="Expense Date"
                     SaveButtonText="@(isEdit ? "Update Expense" : "Add Expense")"
                     ShowCategoryField="true"
                     ShowRecurrenceField="false"
                     Model="@dialogModel"
                     OnCancel="CloseDialog"
                     OnSave="SaveExpense" />
```

### Adhoc Payments Management
```razor
<FinancialItemDialog IsVisible="@showDialog"
                     Title="@(isEdit ? "Edit Payment" : "Add New Payment")"
                     NameLabel="Description"
                     NamePlaceholder="Enter payment description..."
                     DateLabel="Payment Date"
                     SaveButtonText="@(isEdit ? "Update Payment" : "Add Payment")"
                     ShowCategoryField="true"
                     ShowRecurrenceField="false"
                     Model="@dialogModel"
                     OnCancel="CloseDialog"
                     OnSave="SavePayment" />
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
| `RecurrenceOptions` | `List<string>` | `["Monthly"]` | Available recurrence options |
| `Model` | `FinancialItemDialogModel` | `new()` | The data model |
| `OnCancel` | `EventCallback` | - | Callback when cancel is clicked |
| `OnSave` | `EventCallback` | - | Callback when save is clicked |

## FinancialItemDialogModel

### Properties
- `Id` (Guid?) - Entity ID (null for new items)
- `IsEditMode` (bool) - Whether in edit mode
- `Name` (string) - Name/description of the item
- `Currency` (string) - Currency code (default: "USD")
- `Amount` (decimal?) - Monetary amount
- `DateTime` (DateTime) - Date/time value
- `Category` (string?) - Optional category
- `Recurrence` (string) - Recurrence pattern (for bills)
- `IsSaving` (bool) - Loading state during save

### Methods
- `Validate(bool requireName = true)` - Validates the model
- `ClearErrors()` - Clears all validation errors

## Architecture Benefits

1. **DRY Principle**: Eliminates duplicate dialog code across pages
2. **Consistency**: Uniform UI/UX across all financial dialogs
3. **Maintainability**: Single point of change for dialog behavior
4. **Extensibility**: Easy to add new field types or validation rules
5. **Type Safety**: Strongly-typed model with compile-time checking

## File Locations

- **Component**: `src/BudgetExperiment.Client/Components/FinancialItemDialog.razor`
- **Usage Examples**:
  - `src/BudgetExperiment.Client/Pages/FluentBillsManagement.razor`
  - `src/BudgetExperiment.Client/Pages/FluentExpensesManagement.razor`

## Dependencies

- Microsoft.FluentUI.AspNetCore.Components
- Microsoft.AspNetCore.Components (included in _Imports.razor)
