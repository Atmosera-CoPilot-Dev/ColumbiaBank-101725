# Exercise: OrgChart (.NET MAUI)

Build a .NET 8 MAUI OrgChart app using MVVM, validation, services, and automated tests. This README is a rapid Copilot prompt guide. For authoritative requirements (properties, commands, test cases, acceptance criteria, UI identifiers, folder layout), see specs.md.

Tip: Add specs.md to the chat context BEFORE issuing a prompt from the ladder so Copilot honors all constraints
---

Categories overview:
- Outcome: What you will deliver.
- Platform Scope: Initial platform constraints (start narrow for speed).
- Process Workflow: How you iterate every slice.
- Build Order: Recommended implementation sequence.
- Success Criteria: What must be true to call it done.
- Prompt Library: Reusable prompt snippets & refinements.

---

## 1. Outcome – Goal (You Build)
- Employee model + validation
- Mock + HTTP data services
- ViewModel: async load, validation filtering, error mapping
- XAML UI (AutomationIds)
- Unit + basic UI automation tests
- Quality gates (analyzers, formatting)

---

## 2. Platform Scope – Initial Target
Start with Windows only for faster iteration:
- Set TargetFramework (or TargetFrameworks) in orgchart.csproj to  net8.0-windows
- Remove android, ios, maccatalyst, tizen from initial build to reduce complexity and test time
- Defer adding other platforms until core acceptance criteria pass and tests are green

---

## 3. Process Workflow – Working Loop
1. Open specs.md section for the slice
2. Craft a focused prompt (from below)
3. Generate code into correct file/folder
4. Prune usings / fix naming
5. Add/update tests immediately (tests are continuous, not deferred)
6. Run `dotnet test`
7. Refactor, commit, repeat

Keep vertical slices small.

---

## 4. Build Order – Execution Sequence
1. Model + JSON seed (create initial model validation tests immediately)
2. MockDataService
3. Base + EmployeeListViewModel (happy path)
4. MainPage UI bindings
5. Validation & error mapping (expand ViewModel tests)
6. HTTP service + fault tests
7. UI automation
8. Performance (streaming) (optional)
9. Extensibility (optional)

---

## 5. Success Criteria – Acceptance Essentials
- Model validation enforced
- Both data services wired (can switch later to HTTP)
- ViewModel load is async/cancelable, filters invalid, sets ErrorMessage
- MainPage AutomationIds present
- Unit + minimal UI tests green
- No sync-over-async / blocking
- Analyzer warnings resolved
- ValidationErrors populated deterministically
- Initial target framework limited to Windows only (net8.0-windows) for faster baseline; other platforms deferred

For full acceptance checklist & folder map: specs.md (Completion Criteria & Project Structure sections).

---

## 6. Prompt Library – Core Prompt Patterns (with Refinement Examples)

### Model & Data
- "Create Employee model (Id, FirstName, LastName, DepartmentName, HourlyRate decimal(5,2)) with DataAnnotations + Validate() returning IList<ValidationResult>."
- "Generate orgdata.json with 10 employees, HourlyRate 20–50 (2 decimals), sorted by DepartmentName then LastName."

### Services
- "Scaffold IDataService GetEmployeesAsync(CancellationToken) -> IReadOnlyList<Employee>."
- "Implement MockDataService loading Assets/orgdata.json async via System.Text.Json."
- "Implement HttpDataService (injected HttpClient) calling https://localhost:6543/orgdata; handle HttpRequestException, TaskCanceledException, JsonException; validate JSON shape."
- "Add IValidationService + DataAnnotationsValidationService (object + single property validation)."

### ViewModel
- "Create BaseViewModel (INotifyPropertyChanged, SetProperty)."
- "Create EmployeeListViewModel: ObservableCollection<Employee> Employees; ObservableCollection<ValidationResult> ValidationErrors; IsBusy; ErrorMessage; RefreshCommand; supports cancellation; filters invalid employees."
- "Enhance load: distinct catches (HttpRequestException, TaskCanceledException, JsonException) -> friendly ErrorMessage."

### UI (XAML)
- "Build MainPage.xaml: CollectionView (ItemsSource=Employees), Refresh button (RefreshCommand), validation summary (visible when ValidationErrors.Count>0), error banner (ErrorMessage not empty). AutomationIds: EmployeesList, RefreshButton, ErrorBanner, ValidationSummary."

### Performance / Streaming
- "Refactor MockDataService to stream JSON with JsonSerializer.DeserializeAsync and validate incrementally."

### Validation & Error Handling
- "Add ValidateAllEmployees() returning (IReadOnlyList<Employee> valid, IReadOnlyList<ValidationResult> errors)."
- "Add structured logging (success count + validation failure count)."

### Testing (NUnit)
- "Write tests: Employee valid vs missing DepartmentName vs HourlyRate out of range."
- "MockDataService: success + malformed JSON -> JsonException."
- "HttpDataService: success, 500 -> HttpRequestException, malformed JSON, cancellation."
- "EmployeeListViewModel: successful load populates Employees; invalid items recorded; error path sets ErrorMessage."
- "Limit initial TargetFrameworks to net8.0-windows in the .csproj for simpler test runs."

### UI Automation (Appium)
- "Test: wait for EmployeesList; assert expected row count."
- "Test: simulate server error; assert ErrorBanner visible."

### Extensibility (Optional)
- "Add Department filter (Picker + distinct list)."
- "Add AverageHourlyRate computed property."

### Refinement Examples
- "Why does CollectionView not update when Employees modified? Diagnose likely binding / thread issues."
- "Refactor to remove duplication between MockDataService and HttpDataService (focus on deserialization + error mapping)."
- "Generate only NUnit test methods for ViewModel error cases; omit comments."

---

All deeper specifics live in specs.md. Keep prompts explicit (language, target file path, constraints).








