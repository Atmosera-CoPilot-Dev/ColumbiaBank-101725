## OrgChart (.NET MAUI) – Technical Specification

Purpose: Provide a concise, reference?style specification for the lab exercise. This is a learning project, not a production system.
---
### 1. Scope
Build a small .NET 8 MAUI (Windows?only initial) Org Chart viewer demonstrating:
- Model validation (DataAnnotations)
- Two data sources (mock JSON + HTTP)
- MVVM ViewModel with async loading, filtering invalid data, error surfacing
- Basic XAML UI with required AutomationIds
- NUnit unit tests + minimal UI automation (Appium/WinAppDriver)
- Simple logging & structured validation feedback

Out of scope (for core pass): authentication, persistence, caching, localization, advanced theming.

---
### 2. Platform & Framework
- TargetFramework: net8.0-windows (single target for speed)
- Defer Android / iOS / MacCatalyst until after acceptance criteria met
- Nullable enabled; treat most analyzer warnings as errors (recommended but optional for lab if blocking)

---
### 3. Domain Model
Employee
- Properties: EmployeeId (int), DepartmentName (string), FirstName (string), LastName (string), HourlyRate (decimal)
- Validation (indicative):
  - [Required] on all string fields
  - [StringLength] (e.g., 2–50) on names (tune minimally)
  - DepartmentName limited to small known set (HR, QA, Engineering) – enforce via custom check or Regex
  - HourlyRate: [Range(1, 1000)] (lab: realistic 20–50 in seed file)
- Method: IEnumerable<ValidationResult> Validate()
- Serialization: System.Text.Json (default naming; add [JsonPropertyName] only if deviating)

---
### 4. Data Sources
Seed Data (orgdata.json)
- 10 entries, HourlyRate two decimals, sorted by DepartmentName then LastName.

Interfaces & Implementations:
- IDataService
  - Task<IReadOnlyList<Employee>> GetEmployeesAsync(CancellationToken)
- MockDataService
  - Loads orgdata.json (Content: Copy if newer). Guard malformed JSON (JsonException propagates).
- HttpDataService (added after mock + ViewModel tests)
  - GET https://localhost:6543/orgdata
  - Uses injected HttpClient (timeout reasonable, e.g., 10s)
  - Throws: HttpRequestException (network/server), TaskCanceledException (timeout/cancel), JsonException (bad payload)
  - Validate basic JSON shape (array of objects with required fields)

Validation Service:
- IValidationService
  - IEnumerable<ValidationResult> Validate(object instance)
  - IEnumerable<ValidationResult> ValidateProperty(object instance, string propertyName)
- DataAnnotationsValidationService: wraps Validator

---
### 5. ViewModel Layer
BaseViewModel
- Implements INotifyPropertyChanged + protected bool SetProperty<T>(ref T field, T value, string propertyName)

EmployeeListViewModel
- ObservableCollection<Employee> Employees (only valid employees)
- ObservableCollection<ValidationResult> ValidationErrors (invalid employee aggregated results)
- string? ErrorMessage
- bool IsBusy
- ICommand RefreshCommand (async, cancelable)
- Dependencies: IDataService, IValidationService, ILogger (optional simple logging)
- Logic (Refresh):
  1. Clear collections; set IsBusy
  2. Fetch employees (await)
  3. Validate each; add invalid errors to ValidationErrors; add only valid employees to Employees
  4. Catch specific exceptions -> map to friendly ErrorMessage
  5. Finally: IsBusy = false
- CancellationToken support (pass through from command execution token source)

Error Mapping (example – keep terse):
- HttpRequestException -> "Network error retrieving employees."
- TaskCanceledException -> "Request canceled or timed out."
- JsonException -> "Received invalid data format."

---
### 6. UI (MainPage)
Controls (minimum):
- CollectionView ItemsSource=Employees (AutomationId="EmployeesList")
- Button or RefreshView triggering RefreshCommand (AutomationId="RefreshButton")
- Error banner (Label / Border) visible when ErrorMessage not empty (AutomationId="ErrorBanner")
- Validation summary (ListView / CollectionView / simple ItemsControl) bound to ValidationErrors (visible >0) (AutomationId="ValidationSummary")
- Basic styles via ResourceDictionary (optional minimal – focus on function)

No advanced layout required; just ensure binding works and identifiers present.

---
### 7. Error Handling & Validation Rules
Required Behaviors:
- Invalid employees never appear in Employees
- All validation failures appear deterministically in ValidationErrors (do not swallow)
- Distinct exception types produce distinct ErrorMessage strings
- No blocking calls (.Result / .Wait / Task.Run sync bridging)

---
### 8. Performance & Simplicity
- Use async file + HTTP I/O
- Avoid premature optimization
- Optional enhancement: stream JSON in MockDataService with JsonSerializer.DeserializeAsync (list of employees) — only after baseline

---
### 9. Testing Requirements (NUnit & UI Automation)
Model Tests:
- Valid employee passes validation
- Missing DepartmentName fails
- HourlyRate outside expected range fails

MockDataService Tests:
- Successful load returns expected count
- Malformed JSON triggers JsonException

HttpDataService Tests (stub HttpMessageHandler):
- 200 OK valid payload -> expected list
- 500 (or non-success) -> HttpRequestException
- Malformed payload -> JsonException
- Cancellation (trigger token) -> TaskCanceledException

ViewModel Tests:
- Refresh success: Employees populated; ErrorMessage null/empty
- Invalid employees filtered out; ValidationErrors populated
- Service exception sets correct ErrorMessage and leaves Employees empty

UI Automation (minimal – Windows):
- Page loads; EmployeesList item count > 0 (happy path using MockDataService or stub)
- Simulated service error (inject failing IDataService) -> ErrorBanner visible

---
### 10. Logging (Lightweight)
- At minimum log (Info/Debug) counts: valid employee count, invalid count
- Log exception messages at error level before mapping to friendly text

---
### 11. Completion Criteria (Acceptance)
Done when:
- All required properties, validation, and filtering implemented
- Mock + HTTP services function per contract
- Distinct exception mapping implemented
- MainPage AutomationIds present and bindings functional
- Required unit tests & minimal UI automation tests passing
- No analyzer critical warnings (optional to enforce all)
- No synchronous blocking of async calls
- ValidationErrors reliably populated for invalid input
- Windows build (net8.0-windows) succeeds

---
### 12. Extensibility (Optional After Acceptance)
- Department filter (Picker) + derived list of unique departments
- AverageHourlyRate property (decimal?) over valid employees
- Platform expansion to Android/iOS/MacCatalyst
- Incremental streaming + cancellation improvements

---
### 13. Execution (Minimal Recommended Order)
1. Model + seed JSON + model tests
2. MockDataService + tests
3. BaseViewModel + EmployeeListViewModel (happy path) + tests
4. MainPage UI + AutomationIds
5. Validation filtering + error mapping tests
6. HttpDataService + tests
7. UI automation tests
8. Optional enhancements

Keep test coverage lean but meaningful; prioritize correctness over breadth.

---
### 14. Out of Scope
- Persistence beyond in-memory/JSON
- Authentication/authorization
- Complex theming, localization
- Offline sync / caching
- Advanced telemetry / metrics

---
Reference this spec in prompts to keep Copilot responses aligned and concise.
