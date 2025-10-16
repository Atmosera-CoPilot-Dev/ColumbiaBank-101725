# Concurrent Data Structures Lab (with Copilot)

## Lab Goal
Enhance the C# app to complete the implementation of concurrent data processing. The app should read stock data from a CSV file, process it concurrently using multiple consumer tasks, and support cancellation. 

Tip: Add specs.md to the chat context BEFORE issuing a prompt from the ladder so Copilot honors all constraints

---

## Getting Started

- Use Copilot to help you review the code in `Program.cs`, `TradeDayProcessor.cs`, and `TradeDay.cs`.
- Familiarize yourself with the data in `DowJones.csv`.

---

## Suggested Copilot Prompts
Use the prompts similar to the following (one at a time) to guide Copilot in making the required changes. You can copy/paste these into Copilot Chat or use your own wording.

### 0. Understand Current State
- "In 2 short sentences, summarize what the current app does based on Program.cs, TradeDayProcessor.cs, and TradeDay.cs, and explain why adding a producer/BlockingCollection, multiple consumers, result aggregation, and cancellation support is needed. Keep it concise."

### 1. Add a Producer Task
- "Add a BlockingCollection<TradeDay> to TradeDayProcessor and implement a method to generate TradeDay objects from the CSV file, adding them to the collection."
- "Create a Task to run the generator method and start it in the Start method."

### 2. Add Consumer Tasks
- "Add a List<Task<int>> to hold consumer tasks in TradeDayProcessor."
- "Implement a method that consumes TradeDay objects from the BlockingCollection, counts those matching a predicate, and returns the count."
- "Start multiple consumer tasks in the Start method, as specified by the constructor argument."

### 3. Aggregate Results
- "Implement a method to wait for all tasks to finish and sum the results from the consumer tasks. Handle exceptions appropriately."

### 4. Add Cancellation Support
- "Add support for CancellationToken in TradeDayProcessor and its tasks."
- "Implement a Canceller method in Program.cs that cancels processing when the user presses Enter."
- "Update the Start method and processing loops to respect cancellation."

### (Optional) 5. Constructor Validation & Tests
Strengthen the design by validating constructor arguments and adding unit tests.
- "Validate TradeDayProcessor constructor: throw ArgumentOutOfRangeException if numConsumers <= 0."
- "Throw ArgumentNullException if the predicate argument is null."
- "Add a constructor overload test ensuring the default data file is resolved when only numConsumers and predicate are provided."
- "Write MSTest verifying a FileNotFoundException is thrown when a non-existent CSV path is passed."
- "Create tests for happy path: temp CSV with two rows, predicate should count only matching rows."
- "Add a helper method in test class to create a temp CSV with header + provided rows."

Example test assertions:
- Invalid consumer count -> Assert.ThrowsException<ArgumentOutOfRangeException>(...).
- Null predicate -> Assert.ThrowsException<ArgumentNullException>(...).
- Missing file -> Assert.ThrowsException<FileNotFoundException>(...).
- Valid input -> Assert.AreEqual(expected, count, "Should count rows where predicate returns true.");

---

## Tips
- Use Copilot to generate code, but review and test each change.
- If you get stuck, ask Copilot for help with specific errors or refactoring.
- Optional constructor validation & tests reinforce defensive programming and unit testing practices.

---



