# Copilot Instructions (Project-Specific)

Keep suggestions aligned with existing code (C#, .NET Framework 4.8, MSTest).

## Core Code Guidelines
- Target: .NET Framework 4.8.
- Naming: Types/members = PascalCase; locals/params = camelCase.
- Indentation: 4 spaces. Always use braces on separate lines for all control blocks.
- Use `var` only when the type is obvious.
- Keep methods small; extract helpers if they grow.
- Pass `CancellationToken` through long-running loops.
- Dispose I/O resources (`using`).
- Concurrency primitives: `BlockingCollection`, `Task`, `CancellationToken` (no extras).
- Validate inputs early (file path exists, consumer count > 0, predicate not null).
- XML docs for public types/members (optional if time is short).

## Concurrency Pattern
- 1 producer: reads CSV, adds `TradeDay` to a `BlockingCollection`.
- N consumers: take items, apply predicate, count matches.
- Aggregate after all tasks complete; surface faults via `AggregateException`.
- Honor cancellation in producer and consumers.

## Testing (Optional)
- Framework: MSTest `[TestClass]` / `[TestMethod]`.
- Use temp CSV files; always include header: `Date,Open,High,Low,Close,Volume,AdjClose`.
- Pattern: Arrange / Act / Assert.
- Cover: successful count, multi-consumer consistency, cancellation (early stop), missing file (exception), predicate throwing (fault aggregation).

Helper for temp CSV:
```csharp
private string CreateTempCsv(params string[] rows)
{
    var path = Path.GetTempFileName();
    var lines = new List<string> { "Date,Open,High,Low,Close,Volume,AdjClose" };
    lines.AddRange(rows);
    File.WriteAllLines(path, lines);
    return path;
}
```

## Example
```csharp
var processor = new TradeDayProcessor(2, dataFilePath, d => d.Close > d.Open);
var cts = new CancellationTokenSource();
processor.Start(cts.Token);
int matches = processor.GetMatchingCount();
Console.WriteLine($"Matches: {matches}");
```

## Suggested Prompts
- "Implement producer using BlockingCollection in TradeDayProcessor."
- "Add multiple consumer tasks and aggregate counts."
- "Add cancellation support to producer and consumers."
- "Add a test for missing file exception propagation."

## Review Checklist
- Input validation present.
- Cancellation respected in loops.
- All tasks awaited before aggregation.
- Exceptions surfaced (no silent faults).
- Tests (if added) cover core behaviors.
- No unnecessary abstractions or packages.

Keep changes incremental; test after each logical step.
