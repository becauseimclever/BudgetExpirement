# Feature: Calendar-Only UI Simplification

**Created**: 2025-10-10  
**Branch**: `feature/calendar-only-ui`  
**Status**: In Progress (Phase 1 Complete)

## Overview
Simplify the Blazor WebAssembly client UI by removing the Fast Entry page and left-hand navigation menu, leaving only the calendar display as the primary (and only) view.

## Current State Analysis

### Components to Remove
1. **Fast Entry Page** (`Pages/FastEntry.razor`)
   - Full-page transaction entry form
   - Shows recent entries and daily totals
   - ~599 lines of code
   - Route: `/fast-entry`

2. **Left Navigation Menu** (`Layout/NavMenu.razor`)
   - Collapsible navigation with two links:
     - Budget Calendar (/)
     - Fast Entry (/fast-entry)
   - FluentUI NavMenu component
   - ~28 lines of code

3. **Navigation Toggle in MainLayout** (`Layout/MainLayout.razor`)
   - Collapsible navigation toggle button in header
   - Navigation container with transition animations
   - ~56 lines of code

### Components to Keep & Modify
1. **FluentCalendar Page** (`Pages/FluentCalendar.razor`)
   - Primary calendar view at route `/`
   - Already includes dialog-based editing for:
     - Adhoc transactions (`FinancialItemDialog`)
     - Recurring schedules (`UnifiedScheduleDialog`)
   - ~573 lines of code
   - **No changes needed** - already provides full functionality via dialogs

2. **MainLayout** (`Layout/MainLayout.razor`)
   - **REFACTOR**: Simplify to single-column layout
   - Remove navigation toggle button
   - Remove left navigation container
   - Keep header with application title
   - Keep body content area

3. **App.razor** (`App.razor`)
   - **No changes needed** - routing handled by page removal

## Goals
- [ ] Simplify UI to single-purpose calendar view
- [ ] Reduce cognitive load - one primary interaction pattern (calendar + dialogs)
- [ ] Remove redundant data entry paths (Fast Entry duplicates calendar dialog functionality)
- [ ] Clean up routing (single root route)
- [ ] Maintain all existing functionality via calendar's dialog-based editing

## Implementation Plan

### Phase 1: Remove Fast Entry (TDD) ✅ COMPLETED
1. **Remove Fast Entry page** ✅
   - Deleted `Pages/FastEntry.razor` (599 lines removed)
   - No code-behind file existed
   
2. **Update Client Tests** ✅
   - No Fast Entry component tests existed
   - Calendar dialog tests remain intact

**Commit**: `55717d9` - "refactor(client): Remove Fast Entry page (Phase 1)"

### Phase 2: Remove Navigation Menu
1. **Delete NavMenu component**
   - Delete `Layout/NavMenu.razor`

2. **Simplify MainLayout**
   - Remove navigation toggle button from header
   - Remove left navigation container
   - Remove collapse state management (`navCollapsed`)
   - Remove navigation transition styles
   - Update body content to full-width (no left margin)
   - Keep header with app title (static, no toggle)

3. **Update styles**
   - Remove `.nav-container` styles
   - Remove transition animations
   - Simplify layout to single-column

### Phase 3: Cleanup & Verification
1. **Verify routing**
   - Ensure `/` route still works (FluentCalendar)
   - Remove any lingering references to `/fast-entry`

2. **Test all functionality**
   - Calendar displays correctly
   - Add/Edit/Delete adhoc transactions via calendar dialogs
   - Add/Edit/Delete recurring schedules via calendar dialogs
   - All calendar interactions work without navigation

3. **Code cleanup**
   - Remove unused imports
   - Run `dotnet format` with analyzers
   - Ensure no StyleCop violations

4. **Documentation**
   - Update README if it references Fast Entry
   - Update any screenshots or UI documentation

## Technical Considerations

### Layout Changes
**Before** (MainLayout structure):
```
FluentLayout
├─ FluentHeader (with nav toggle button)
└─ FluentBodyContent
   └─ FluentStack (Horizontal)
      ├─ NavMenu (collapsible, 60-250px width)
      └─ Body content (flex: 1, with transition)
```

**After** (simplified MainLayout):
```
FluentLayout
├─ FluentHeader (static title, no buttons)
└─ FluentBodyContent
   └─ Body content (full width)
```

### Routing Impact
- **Before**: Two routes (`/`, `/fast-entry`)
- **After**: Single route (`/`)
- **No** `NotFound` impact - still handled by `App.razor`

### Functionality Preservation
All Fast Entry capabilities already exist in calendar view:
- ✅ Add adhoc transactions → Calendar day click → "Add New Transaction" dialog
- ✅ Edit transactions → Click transaction in calendar → Edit dialog
- ✅ Delete transactions → Edit dialog → Delete button
- ✅ View recent entries → Calendar displays all transactions by date
- ✅ Daily totals → Calendar day cells show running totals

**No functionality loss** - Fast Entry was a redundant input path.

## Testing Strategy

### Manual Testing Checklist
- [ ] Run API (`dotnet run --project c:\ws\BudgetExpirement\src\BudgetExperiment.Api\BudgetExperiment.Api.csproj`)
- [ ] Navigate to root (`http://localhost:5099`)
- [ ] Verify calendar displays correctly (full width, no navigation)
- [ ] Test add adhoc transaction via day click
- [ ] Test edit adhoc transaction via transaction click
- [ ] Test delete adhoc transaction via edit dialog
- [ ] Test add recurring schedule via calendar controls
- [ ] Test edit recurring schedule via schedule click
- [ ] Test delete recurring schedule via edit dialog
- [ ] Verify no broken links or navigation errors
- [ ] Verify no console errors in browser dev tools

### Component Tests (if needed)
- MainLayout renders without navigation
- FluentCalendar dialogs still function
- No Fast Entry references remain

## Migration Notes
**Breaking Changes**: None - this is purely a UI simplification.  
**Data Impact**: None - no database or API changes.  
**Deployment**: Standard client-only deployment (API serves updated wwwroot).

## Files to Modify

### Delete
- `src/BudgetExperiment.Client/Pages/FastEntry.razor`
- `src/BudgetExperiment.Client/Layout/NavMenu.razor`

### Modify
- `src/BudgetExperiment.Client/Layout/MainLayout.razor` (simplify layout)

### Verify No Impact
- `src/BudgetExperiment.Client/App.razor` (routing still works)
- `src/BudgetExperiment.Client/Pages/FluentCalendar.razor` (no changes needed)
- All dialog components (already functional)

## Success Criteria
- [x] Planning document created
- [ ] Feature branch created (`feature/calendar-only-ui`)
- [ ] Fast Entry page removed
- [ ] Navigation menu removed
- [ ] MainLayout simplified to single-column
- [ ] All calendar functionality verified working
- [ ] No StyleCop violations
- [ ] No console errors
- [ ] Code formatted with `dotnet format`
- [ ] Manual testing checklist completed
- [ ] PR ready for review

## Future Enhancements (Out of Scope)
- Add keyboard shortcuts for common actions
- Implement quick-add via calendar header
- Add calendar zoom/density controls

## Notes
- **TDD**: No new domain/application logic = no new unit tests required (pure UI removal)
- **Integration Tests**: API tests unaffected (no API changes)
- **Client Tests**: Update if Fast Entry component tests exist
- **Deployment**: CI/CD pipeline builds and publishes automatically on merge to `main`

---
**Last Updated**: 2025-10-10
