# Copilot Instructions (Project-Specific)

Keep suggestions aligned with existing code (C#, .NET Framework 4.8, MSTest). 

## Code Guidelines
- Target: .NET Framework 4.8 (no new SDK / async streaming abstractions unless needed).
- Naming: Classes / methods / properties = PascalCase; locals / parameters = camelCase; constants = PascalCase.
- Indentation: 4 spaces.
- Always wrap bodies of if / else / for / foreach / while / do / switch case clauses in braces, even for a single statement. Opening brace on a new line (match current files).
- Use `var` only when the type is obvious from the right-hand side.
- Keep methods small; extract helpers if a method grows large.
- Pass `CancellationToken` through long-running or loop code (see Program.cs usage).
- Dispose I/O resources (`using` or `try/finally`).
- Avoid introducing new external packages unless required.
- Prefer clarity over cleverness; keep concurrency primitives minimal (BlockingCollection, Task, CancellationToken).
- Validate inputs early (file paths, consumer counts, null predicates).
- XML Docs: All public (and internal) classes, interfaces, enums, structs


## Concurrency Pattern
- One producer reads CSV -> pushes TradeDay items.
- N consumers pull items -> apply predicate -> count matches.
- Aggregate counts only after all tasks complete; surface exceptions via AggregateException.
- Honor cancellation in producer & consumers (periodically check token or collection completion state).

## Testing Guidelines (MSTest)
- Use `[TestClass]` and `[TestMethod]` (do NOT switch to NUnit/xUnit).
- Follow Arrange / Act / Assert sections (commented where helpful).
- Use temp files for data. Clean up after tests.
- Always include the CSV header line in every temp test file because production parsing unconditionally reads and discards the first line (`Date,Open,High,Low,Close,Volume,AdjClose`). Omitting it will cause the first data row to be skipped.
- Prefer real temp file I/O over mocking for CSV parsing to keep tests close to production behavior; only introduce mocking when:
  - The dependency is non-deterministic (e.g., time, randomness),
  - The dependency is slow or external (network, database),
  - Or behavior branching would otherwise be untestable.
- Do not mock simple data objects or .NET primitives; avoid adding interfaces solely to enable mocking unless there is clear variability (e.g., future alternate data source).
- If later an alternate data source (stream, API) is needed, introduce an abstraction like `ITradeDaySource` and mock that interface—keep its surface minimal.
- Cover: valid processing, zero consumers, null predicate, cancellation (future), missing file error propagation.
- Assertions should include a message clarifying the expectation.

## Example: Starting Processing
```csharp
var processor = new TradeDayProcessor(2, dataFilePath, d => (d.Close - d.Open) / d.Open > 0.05);
var cts = new CancellationTokenSource();
processor.Start(cts.Token);
int matches = processor.GetMatchingCount();
Console.WriteLine($"Matches: {matches}");
```

## Example: MSTest Pattern
```csharp
[TestMethod]
public void Predicate_FindsExpectedMatches()
{
    // Arrange
    var file = CreateTempCsv("2024-01-02,100,110,95,108,1000,108");
    var processor = new TradeDayProcessor(1, file, d => d.Close > d.Open);

    // Act
    processor.Start();
    var count = processor.GetMatchingCount();

    // Assert
    Assert.AreEqual(1, count, "Should count rows where Close > Open.");
}
```

## Suggested Copilot Prompts
- "Add cancellation checks inside the consumer loop in TradeDayProcessor."
- "Refactor TradeDayProcessor to validate constructor arguments (file path, consumer count)."
- "Add a test verifying AggregateException contains FileNotFoundException for missing file."

## Review Checklist
- Input validation present?
- Cancellation respected?
- All tasks awaited before aggregating results?
- Tests added/updated for new behavior?
- No unused usings or unnecessary abstractions?

Keep file edits incremental and test after each logical change.
