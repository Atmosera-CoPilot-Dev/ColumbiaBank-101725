# Exercise 9 – Detailed Specifications & Constraints (specs.md)

This document contains the authoritative specifications, constraints, quality goals, and detailed guidance for the WPF Address Book refactor & enhancement exercise. Load this file into Copilot Chat (add to context) before issuing prompts from the ladder in README.md so all constraints are honored.

## 1. Objectives
Refactor and enhance the legacy WPF MVVM address book sample to achieve:
- Robustness (error handling, validation, async safety)
- Testability (repository abstraction, deterministic behaviors, unit tests)
- Clarity & Maintainability (naming, reduced coupling, documentation)
- UX Improvements (validation feedback, explicit save, non-blocking operations)

## 2. Baseline Investigation Scope
Files of interest:
- Window1.xaml / Window1.xaml.cs
- ViewModels/*.cs
- Model/*.cs
- JulMar/*.cs (command + base VM helpers)
- addressbook.xml (data file)

Outcomes expected from baseline analysis:
- Architecture summary (patterns, responsibilities, data flow)
- Identification of synchronous/blocking I/O
- Inventory of magic strings / hard-coded paths
- Validation gaps and potential user-facing issues
- Coupling and layering issues
- Disposal / lifetime pattern assessment

## 3. Known Initial Issues / Risks (Seed List)
- Nullable reference types disabled; risk of NullReferenceException.
- No defensive file I/O (missing, locked, malformed XML) -> crashes or data loss.
- Blocking synchronous load/save on UI thread -> potential UI freeze.
- Save only on Dispose; no mid-session explicit persistence.
- No model or UI validation (Name, Email, Phones, ZipCode not validated).
- Magic strings in property change notifications.
- Command CanExecute not reevaluated on selection changes (RemoveCommand risk).
- Tight coupling to static ContactCardManager & file path.
- XML deserialization assumes full schema (no defaults/resilience).
- No automated tests -> unsafe refactor surface.
- Missing XML documentation; inconsistent coding standards.
- Mutable data objects without invariants or trimming/normalization.
- No dirty tracking; broad unsaved changes prompt only.
- Inconsistent email/phone formatting; trimming absent.
- No repository abstraction obstructing future persistence strategies.

You must expand this list with maintainability, scalability, and UX concerns (e.g., lack of separation for validation logic, absence of logging, potential threading race conditions if async added naively, etc.).

## 4. Target Refactor Themes
1. Safety: Introduce nullability, guards, tests.
2. Reliability: Graceful file I/O handling, resilient parsing, clear user feedback.
3. UX: Validation visibility, explicit save, non-blocking operations, search/filter.
4. Maintainability: Abstractions (IContactRepository), naming consistency, documentation, reduced coupling.

## 5. Repository Abstraction
Interface (example shape):
- Task<IReadOnlyList<ContactCard>> LoadAsync(string path, CancellationToken ct = default)
- Task SaveAsync(string path, IEnumerable<ContactCard> contacts, CancellationToken ct = default)
Error Contract:
- Missing file -> return empty list (no exception) unless path invalid
- Malformed XML -> throw RepositoryException (wrap inner)
- I/O issues -> throw RepositoryException with context message

XmlContactRepository Guidelines:
- Use async-friendly patterns (Stream + XmlReader/Writer). If underlying APIs lack async, consider Task.Run with justification.
- Ensure deterministic resource disposal (using statements).
- Avoid blocking (.Result / .Wait()).

## 6. Validation Strategy
Use DataAnnotations OR a concise custom rule set:
- ContactCard: Name (Required, trim > 0), Email (Regex simple pattern, optional but if present must match), ZipCode (alphanumeric or numeric pattern), Phones (optional; normalize digits + separators), Address fields (optional but trimmed).
- Provide per-property error surfacing via INotifyDataErrorInfo.
- Revalidate on property change; short-circuit unchanged values.
- Maintain ErrorsChanged events to update UI immediately.

## 7. Dirty Tracking
Requirements:
- IsDirty flag at ViewModel-level; per-contact modifications set flag.
- Reset IsDirty after successful save.
- New contact creation sets IsDirty.
- On close: prompt only if IsDirty.
- Optionally track per-contact dirty state for advanced UX (not mandatory).

## 8. Commands & UI Behavior
- Add SaveCommand (CanExecute => IsDirty && !IsBusy && no current validation errors that block save).
- RemoveCommand reevaluates CanExecute when SelectedCard changes.
- AddContactCommand sets selection to new contact.
- IsBusy boolean for async operations; disable relevant commands during operations.

## 9. Async Operations
- Load and Save should be fully async; update IsBusy.
- Catch exceptions; update a Status or ErrorMessage property (INotifyPropertyChanged) for binding.
- Never block UI thread; rely on async/await.

## 10. Testing (NUnit)
Minimum Coverage:
- XmlContactRepository: roundtrip save/load; missing file returns empty; malformed XML => RepositoryException.
- Validation: invalid email, empty required Name, Zip pattern failure, trimming of whitespace-only input.
- Dirty tracking: edit property -> IsDirty true; save resets; no changes -> IsDirty remains false after load.
- Commands: RemoveCommand CanExecute false when null selection; SaveCommand gating logic.

Testing Guidelines:
- Use Temp file paths (Path.GetTempFileName + File.Delete after).
- Avoid reliance on UI components; test ViewModels and repository logic.

## 11. Coding Standards (Reinforce)
- CamelCase for locals/method params; PascalCase for public members.
- Private fields prefixed with _.
- XML docs on all public types/members.
- Use nameof instead of string literals for property names.
- Use var for evident types.
- Trim and normalize input in setters (avoid surprising mutation loops by guarding equality before set).
- Avoid deeply nested logic; extract methods.

## 12. Error Handling & Logging
- Wrap repository exceptions with user-friendly messages (e.g., "Could not load contacts: ...").
- Surface a concise message to UI; log (Console or Debug) detailed messages for dev context.
- Avoid swallowing exceptions silently.

## 13. Performance & Resilience
- Do not re-serialize unchanged data unnecessarily (optional optimization: skip save if not dirty).
- Use streaming XML (avoid loading entire DOM if large growth expected).
- Guard against large file scenarios with cancellation token support (stretch goal).

## 14. Optional Enhancements (Stretch)
- JsonContactRepository (System.Text.Json) implementing same interface.
- Recent files MRU list with persistence.
- Undo/Redo via memento pattern.
- Theming (ResourceDictionaries, Light/Dark toggle).
- Async file watcher to detect external modifications (FileSystemWatcher) -> prompt reload.

## 15. Quality Checklist (Completion Criteria)
- All magic strings replaced with nameof.
- Nullable context enabled; warnings addressed or pragmas justified.
- Repository abstraction decouples ViewModels from file details.
- Load/Save async & non-blocking; IsBusy used.
- Robust error handling with user feedback.
- Validation integrated; invalid fields reflected in UI (INotifyDataErrorInfo) or collected.
- Commands reflect state accurately (CanExecute updates).
- Dirty tracking prevents data loss; targeted save behavior implemented.
- Unit tests pass; no flaky tests; coverage of critical behaviors (repository, validation, dirty tracking, commands).
- XML documentation present on public API.
- No unused usings or dead code; consistent formatting.
- No synchronous waits on async tasks.
- Input trimming & normalization implemented.
- Clear final audit (FINAL_NOTES.md) summarizing changes and residual gaps.

## 16. Prompt Hygiene (Reminders)
- Ask for explanation before code when uncertain.
- Narrow prompts to a single file or method when refining.
- Review generated code; enforce standards and constraints manually.
- Iterate: Inspect -> Adjust prompt -> Re-run.

## 17. Deliverables
- Updated source code with refactors and enhancements.
- (Optional) Tests directory with NUnit tests.
- FINAL_NOTES.md summarizing refactors, rationale, remaining improvement ideas.
- Documentation of intentional omissions (e.g., skipped stretch goals) with justification.

## 18. Final Audit Process
1. Run tests; capture results.
2. Compare implementation vs. Quality Checklist.
3. Generate audit prompt: "Audit solution against specs.md Quality Checklist; enumerate gaps with remediation actions." 
4. Address gaps or document deliberate deferrals in FINAL_NOTES.md.

---
Load this specs.md in Copilot Chat context before using the prompt ladder in README.md to ensure compliance. Proceed incrementally and keep commits small and purposeful.
