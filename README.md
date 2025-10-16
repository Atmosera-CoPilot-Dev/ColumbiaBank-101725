# GitHub Copilot Workshop for C# Developers

## Exercises
- **ex1-concurrency**: Two focused labs: (a) a TaskCompletionSource-based single-message producer/consumer refactor for SOLID design ([ex1-concurrency/ex1a-producer-consumer/README.md](ex1-concurrency/ex1a-producer-consumer/README.md)); (b) a multi-consumer pipeline using BlockingCollection, aggregation, validation, and cancellation ([ex1-concurrency/ex1b-concurrent-data-structures/README.md](ex1-concurrency/ex1b-concurrent-data-structures/README.md)).
---
- **ex2-refactor**: Average Salary Refactor – turns a naive inline implementation into a small domain with guarded entity creation, decimal precision, nullable average semantics, explicit exception contract, XML docs, and a comprehensive prompt ladder ([ex2-refactor/README.md](ex2-refactor/README.md)).
---
- **ex3-template-method-pattern**: Implements and evolves a reporting workflow via the Template Method pattern, then (optional) introduces a formatting Strategy layer for extensible output (text / JSON / etc.). Includes refactor + testing prompt ladder ([ex3-template-method-pattern/README..md](ex3-template-method-pattern/README..md)).
---
- **ex4-test-singleton**: Progressive hardening of a naive singleton logger through phases: baseline analysis, configurability, resource management, thread safety, log levels, error handling, DI abstraction, optional async queue, and polish ([ex4-test-singleton/README.md](ex4-test-singleton/README.md)).
---
- **ex5-transform**: ByteFactory component with bounded storage, non‑mutating XOR + delegate transforms, validation + truncation rules, NUnit test ladder, and optional streaming extension prompts ([ex5-transform/README.md](ex5-transform/README.md)).
---
- **ex6-assembly-line**: In‑memory FactoryAssemblyLine implementing capacity/station invariants, active/inactive tracking, processing time queries, and optional cached aggregation optimizations ([ex6-assembly-line/README.md](ex6-assembly-line/README.md)).
---
- **ex7-rest**: Inventory Web API (.NET 8, SQLite + EF Core) with migrations, DTO mapping, CRUD expansion, global error handling (ProblemDetails), pagination/filter/sort roadmap, concurrency tokens, CI, and OpenAPI client generation prompts ([ex7-rest/README.md](ex7-rest/README.md)).
---
- **ex8-gui**: .NET MAUI org chart app: MVVM (models, services, validation service), mock + HTTP data sources, async loading, error resilience, performance notes, NUnit + Appium UI tests, and extensibility checklist ([ex8-gui/Readme.md](ex8-gui/Readme.md)).
---
- **ex9-wpf**: Legacy WPF Address Book refactor: architecture analysis, risk expansion, repository + async persistence abstraction, validation, dirty tracking, command correctness, unit testing, and quality audit prompts ([ex9-wpf/README.md](ex9-wpf/README.md)).
---
- **diy (healthcare / banking / manufacturing)**: Three green‑field DIY scenarios with their own specs prompting full design + implementation + tests:
  - Healthcare CSV normalizer (streaming parse, validation, masking, metrics) ([diy/healthcare/README.md](diy/healthcare/README.md))
  - Bank transaction monitoring core (atomic ledger, rules, suspicion detection, summaries) ([diy/banking/README.md](diy/banking/README.md))
  - Manufacturing simulation (seeded deterministic production + QC loop, metrics, utilization) ([diy/manufacturing/README.md](diy/manufacturing/README.md))
  See umbrella overview ([diy/README.md](diy/README.md)).
---



