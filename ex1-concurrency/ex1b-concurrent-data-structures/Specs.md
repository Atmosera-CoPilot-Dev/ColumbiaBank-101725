# Concurrent Data Structures Enhancement Specs

This document enumerates concrete engineering requirements for enhancing the app to perform concurrent stock data processing with producer / consumer tasks and cancellation.

---
## 1. Data Model
- TradeDay class represents one CSV row with properties: Date (DateTime), Open, High, Low, Close, Volume (long/ulong/int64), AdjClose (decimal/double). Types should match existing implementation; add parsing helper if missing.
- Parsing: skip header. For each non-empty line split by comma. Trim values. Handle malformed numeric/date fields by throwing or optionally logging and skipping (choose consistent strategy; initial spec: throw).

## 2. Processor Responsibilities (TradeDayProcessor)
Provide a class encapsulating the full workflow.

### 2.1 Construction
Inputs:
- int consumerCount (>=0). If 0 no consumers run.
- string csvPath (can be relative or absolute).
- Predicate<TradeDay> matchPredicate (nullable; if null no rows match).
Optionally:
- CancellationToken (or provide SetCancellationToken method later) to support cooperative cancellation.
Validate: store values; do not perform I/O in constructor.

### 2.2 Internal State
Add fields:
- BlockingCollection<TradeDay> _queue (bounded or unbounded; start unbounded; consider capacity constant for back-pressure optional stretch goal).
- Task _producerTask.
- List<Task<int>> _consumerTasks.
- CancellationToken _cancellationToken (default CancellationToken.None) and CancellationTokenSource if processor owns lifetime.
- List<Exception> _exceptions (thread-safe collection or capture via Task.WhenAll).
- bool _started flag.

### 2.3 Public API
- void Start(): idempotent guard; create producer and consumer tasks; start them.
- int GetMatchingCount(): blocks until all tasks complete, aggregates consumer results, propagates errors via AggregateException; returns total matches (0 if predicate is null or consumerCount == 0).
- void Cancel(): calls underlying CancellationTokenSource.Cancel() (only if processor created the source).
- Task<int> GetMatchingCountAsync(): async version (optional enhancement / future requirement).

## 3. Producer Logic
Method: Produce()
Steps:
1. Open csvPath with File.OpenText; on FileNotFound / DirectoryNotFound propagate exception.
2. Read and discard header line.
3. While not EndOfStream and not cancellation requested:
   - Read line; skip empty lines.
   - Parse to TradeDay; on parsing error throw (caught later) OR skip with log (choose throw for clarity).
   - Add to _queue (BlockingCollection.Add). Respect cancellation: if cancellation requested break.
4. CompleteAdding() in finally block so consumers finish even if exception occurs.
5. Exceptions bubble so they become Task faulted.

## 4. Consumer Logic
Method: int Consume()
Steps:
1. Initialize local count = 0.
2. foreach (var trade in _queue.GetConsumingEnumerable(cancellationToken)):
   - If cancellation requested break.
   - If predicate != null and predicate(trade) increment count.
3. Return count.
Errors: Allow predicate exceptions to bubble (fault task).

## 5. Task Startup (Start)
- Guard against multiple calls (throw InvalidOperationException or ignore subsequent calls).
- Instantiate _producerTask = Task.Run(Produce, cancellationToken).
- For i in [0, consumerCount): create Task<int> Task.Run(Consume, cancellationToken); add to _consumerTasks.
- If consumerCount == 0 still start producer so file is read then queue completed (consumers not required; GetMatchingCount returns 0 after producer completes unless errors).

## 6. Aggregation (GetMatchingCount)
Process:
1. Ensure Start() has been called (throw if not).
2. Wait for producer: Task.WaitAll(producerTask) (but we can aggregate after consumers finish too).
3. Wait for all consumer tasks: Task.WhenAll or Task.WaitAll.
4. If any task faulted gather exceptions (producer + consumers) into AggregateException and throw.
5. Sum Task<int>.Result for all consumers (only if no exceptions). Return total.
6. Support cancellation: if OperationCanceledException encountered and cancellation requested, rethrow OperationCanceledException (not wrapped) OR wrap; choose to propagate as-is.

## 7. Cancellation Support
- Provide overload constructor accepting CancellationToken or internally create CancellationTokenSource.
- All blocking calls (_queue.GetConsumingEnumerable) use the token.
- On cancellation: producer stops reading; CompleteAdding still called; consumers exit early.
- GetMatchingCount: if cancellation triggered before completion, throw OperationCanceledException (from awaited task) so caller can handle.

## 8. Error Handling
- File/IO exceptions propagate via producer task; surfaced as AggregateException in GetMatchingCount.
- Parsing exceptions likewise.
- Multiple exceptions: user receives AggregateException.Flatten().
- If both cancellation and other faults occur, prioritize OperationCanceledException if explicitly requested; otherwise aggregate.

## 9. Thread Safety
- Public methods other than Start and GetMatchingCount should be thread safe if added later.
- Use Interlocked.Exchange or simple lock for _started flag.

## 10. Performance Considerations
- Optionally set a bounded capacity (e.g., 10_000) for _queue to prevent uncontrolled memory growth on large CSV.
- Avoid unnecessary allocations in parsing (reuse buffers optional).

## 11. Logging (Optional / Minimal)
- Minimal Console.WriteLine lines for start/stop events and cancellation detection (can be toggled by a verbosity flag; not mandatory now).

## 12. Program.cs Integration
- Parse args or hardcode path to DowJones.csv.
- Instantiate TradeDayProcessor with desired consumer count (e.g., Environment.ProcessorCount - 1 or fixed).
- Start processor.
- Spawn Canceller Task: waits for Enter key (Console.ReadLine) then calls processor.Cancel(). Display message.
- Call GetMatchingCount (sync) inside try/catch to handle AggregateException and OperationCanceledException; print result or cancellation message.

## 13. Testing Strategy 
Add/verify tests:
- Successful count with known CSV and predicate.
- Null predicate returns 0.
- Zero consumers returns 0 even with data.
- Missing file triggers AggregateException containing FileNotFound / DirectoryNotFound.
- Cancellation test: large synthetic CSV; cancel early => expect OperationCanceledException.
- Faulting predicate: throw inside predicate; expect AggregateException with inner exception from predicate.

## 14. Extensibility Hooks (Future)
- Expose async enumeration of matching rows.
- Support multiple predicates or strategies.
- Add statistics (total rows processed, elapsed time).

## 15. Definition of Done
- All specs above implemented (Sections 1–8 mandatory).
- Existing and new tests pass.
- No unhandled task exceptions (Verify by not triggering debugger / finalizer warnings).
- Supports graceful cancellation.

---
Use this spec instead of step-by-step prompt list to implement enhancements systematically.
