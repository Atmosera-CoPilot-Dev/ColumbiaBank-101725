# Producer-Consumer Exercise

The code is a simple producer-consumer example using `Task` and `TaskCompletionSource`.

The goal is to gain experience using Copilot Chat features effectively (Ask or Agent mode). This lab intentionally keeps scope small: understand the code, refactor for SOLID, and build/run. No testing tasks and no advanced / troubleshooting sections.

> Tip: Add `specs.md` to the chat context BEFORE issuing a prompt from the ladder so Copilot honors all constraints.

## Core Lab Tasks
- Use Copilot to generate a .gitignore file for a C# .NET 8 console project.
- Ask Copilot to explain the existing code and the purpose of using `Task` and `TaskCompletionSource`.
- Refactor: separate implementation into appropriate interfaces/classes under namespace `ProduceConsumer` (each type its own file) following SOLID.
- Add (optional) cancellation support suggestion for the producer via `CancellationToken`.
- Build and run the code to ensure it still works.

## Suggested Copilot Prompts
(Use, adapt, or iterate on these.)

### 1. Git & Project Setup
- "Generate a .gitignore suitable for a .NET 8 C# console project with bin/ obj/ ignored."  
- "Review this .gitignore and tell me if anything is missing for typical C# tooling (Rider, VS, VS Code)."  

### 2. Understanding the Existing Code
- "Explain the purpose of TaskCompletionSource<string> in this Program.cs producer-consumer example."  
- "What problem does TaskCompletionSource solve compared to returning a Task.Delay(...).ContinueWith(...) chain?"  
- "Walk through the execution order of producer.ProduceAsync() and consumer.ConsumeAsync() and where the await suspension happens."  
- "List potential race conditions or misuse scenarios with TaskCompletionSource in this code."  

### 3. Applying SOLID & Refactoring
- "Refactor this producer-consumer code into separate files with namespace ProduceConsumer following SOLID. Propose interfaces first (no implementation yet)."  
- "Given the proposed interfaces, generate clean implementations without changing behavior."  
- "Ensure Dependency Inversion: the consumer should depend on an abstraction for receiving messages. Suggest modifications."  
- "Add XML documentation comments to each public interface and class in the refactored version."  
- "Suggest how to make the producer cancellable via CancellationToken."  

### 4. Interface & Design Deep Dives (Optional Exploration)
- "Propose an IMessageProducer and IMessageConsumer interface with async methods and describe their responsibilities in one sentence each."  
- "Suggest how to extend the design to support multiple consumers waiting on the same message."  
- "Show two alternative designs: (a) TaskCompletionSource-based, (b) Channel-based using System.Threading.Channels."  

### 5. Build, Run & Validation
- "Give the exact dotnet CLI commands to build and run this console app in Release mode."  
- "Suggest a watch mode command to rebuild rapidly while refactoring."  
- "How can I add simple logging (Microsoft.Extensions.Logging) with minimal setup in a console app?"  

### 6. Enhancements & Extensions (Optional)
- "Enhance the design to support producing multiple messages; outline options (queue, Channel, IAsyncEnumerable)."  
- "Refactor to use Channel<string> instead of TaskCompletionSource. Provide code."  
- "Demonstrate an async iterator (IAsyncEnumerable<string>) version of the producer with a consumer that enumerates."  
- "Add cancellation support to both producer and consumer and show usage in Main."  
- "Introduce a timeout on the consumer side and show how to surface a friendly error message."  

---
**Refinement Tip:** After any answer ask: "Suggest improvements" or "Provide an alternative approach" to refine further.






