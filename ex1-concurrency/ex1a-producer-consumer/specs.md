# Producer-Consumer Lab Enhancement Specifications

## 1. Scope
Refactor the existing single-file example (Program.cs) into a small, SOLID-aligned structure while preserving observable behavior:
- A producer asynchronously provides a single string message after a simulated delay.
- A consumer asynchronously awaits that message and writes it to the console.

## 2. Current Behavior Summary
- `Producer.ProduceAsync()` performs a 5s delay then calls `TaskCompletionSource<string>.SetResult("Message from producer...")`.
- `Consumer.ConsumeAsync()` awaits the associated `Task<string>` and prints the message.
- `Program.Main` creates the `TaskCompletionSource`, instantiates both classes, starts both tasks, and awaits `Task.WhenAll`.

## 3. Refactoring Requirements
Refactor into the namespace `ProduceConsumer` with one public type per file. Maintain functional parity.

### 3.1 Interfaces
Define at minimum:
- `IMessageProducer`: Exposes `Task ProduceAsync(CancellationToken cancellationToken = default)`.
- `IMessageConsumer`: Exposes `Task ConsumeAsync(CancellationToken cancellationToken = default)`.
- (Optional) `IMessageChannel` abstraction if you wish to decouple producer and consumer from the raw `TaskCompletionSource` / future transport.

### 3.2 Implementations
- `TaskCompletionMessageChannel` (if created) encapsulates a single-use `TaskCompletionSource<string>`.
  - Provide: `Task<string> MessageTask { get; }` and a method like `bool TryPublish(string message)`.
  - Guard against double completion (ignore or return false on second attempt).
- `Producer`: Implements `IMessageProducer`, depends on abstraction (`IMessageChannel` or directly uses a provided `TaskCompletionSource<string>` via constructor injection).
- `Consumer`: Implements `IMessageConsumer`, depends only on an abstraction exposing a `Task<string>` (e.g., `IMessageChannel`).
- `Program`: Wires dependencies manually (no DI container required).

### 3.3 SOLID Alignment
- SRP: Each class has one reason to change (producing, consuming, or message transfer abstraction).
- OCP: Ability to introduce an alternative channel (e.g., Channels API) without modifying consumer/producer code.
- LSP: Alternate producers/consumers should be swappable through interfaces.
- ISP: Keep interfaces minimal (only async methods required for their roles).
- DIP: High-level logic (`Program`, `Consumer`) depends on abstractions not concrete `TaskCompletionSource`.

### 3.4 Naming & Structure
Suggested files:
- `ProduceConsumer/IMessageProducer.cs`
- `ProduceConsumer/IMessageConsumer.cs`
- `ProduceConsumer/IMessageChannel.cs` (optional)
- `ProduceConsumer/TaskCompletionMessageChannel.cs` (optional)
- `ProduceConsumer/Producer.cs`
- `ProduceConsumer/Consumer.cs`
- `Program.cs` (updated to use the new types)

### 3.5 Documentation
Add XML documentation comments for all public interfaces and classes (summary + parameter descriptions).

## 4. Optional Enhancements
You may implement any subset; keep each isolated (avoid breaking base scenario).

### 4.1 Cancellation Support
- Accept an external `CancellationToken` in `ProduceAsync` and `ConsumeAsync`.
- During production delay, pass the token to `Task.Delay`.
- If canceled before completion, set a canceled task instead of a successful result (e.g., do nothing and let consumer await forever? Better: expose a `TryCancel()` on channel to fault or cancel). Choose one approach and document it.

### 4.2 Multiple Messages (Exploratory Only)
Sketch (no full implementation required) one of these pathways:
- Replace single `TaskCompletionSource` with `Channel<string>` (bounded or unbounded) and loop in producer.
- Expose `IAsyncEnumerable<string>` from producer; consumer `await foreach` processes.
- Introduce an in-memory queue with a signaling mechanism (e.g., `SemaphoreSlim`).
Document trade-offs briefly (ordering, backpressure, single vs multi-consumer coordination).

### 4.3 Alternative Transport Implementation
Implement a second channel type using `System.Threading.Channels.Channel<string>` and demonstrate minimal `Program` changes (commented example code block is sufficient inside the file).

### 4.4 Timeout Handling (Optional)
- Demonstrate a consumer-side timeout using `CancellationTokenSource` with `TimeSpan`.
- On timeout, print a friendly message instead of unhandled exception.

## 5. Build & Run
Commands (reference only):
- Restore: `dotnet restore`
- Build (Debug): `dotnet build`
- Build (Release): `dotnet build -c Release`
- Run: `dotnet run`
- Continuous compile while refactoring: `dotnet watch run`

## 6. Acceptance Criteria
- Refactored code compiles under .NET 8.
- Original behavior preserved: after ~5s the message prints exactly once.
- Interfaces introduced and implemented as specified.
- No unhandled exceptions in normal execution.
- Optional enhancements (if implemented) are documented inline or in comments.

## 7. Non-Goals
- Unit tests (explicitly out of scope for this simplified lab).
- Performance optimization beyond structural clarity.
- Production-ready resiliency (retries, logging frameworks) unless added as optional demonstration.

## 8. Suggested Order of Work
1. Extract interfaces (files + namespace).
2. Introduce channel abstraction (optional) and update Producer/Consumer.
3. Move classes to separate files; update Program.
4. Add XML docs.
5. Add cancellation (optional).
6. Explore alternative transport (optional).
7. Build & run to verify behavior.

---

