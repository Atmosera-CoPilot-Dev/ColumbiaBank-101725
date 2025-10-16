## Assembly Line Simulation 

## Overview
This project simulates a factory assembly line with multiple stations. Each station can be started or stopped, and the assembly line can manage these stations while providing various statistics about their states and processing times.	 
Implement `FactoryAssemblyLine` so all tests pass. Keep implementation clear and lean. As is, all but one test will fail.

Tip: Add specs.md to the chat context BEFORE issuing a prompt from the ladder so Copilot honors all constraints

### Goal
Provide a correct implementation of the interface in `IFactoryAssemblyLine` used by the existing NUnit tests.

### Workflow Prompts 
1. Summarize the specification (from specs.md) focusing on responsibilities and invariants (capacity, counts, exception types).
2. Create the private Station model inside FactoryAssemblyLine.
3. Implement AddStation / RemoveStation with validation & exceptions.
4. Implement StartAssembly / StopAssembly (treat repeat calls as idempotent no-ops; tests do not expect exceptions on re-start / re-stop).
5. Add query methods: GetProcessingTime, IsStationActive, GetNumStations, GetNumActiveStations, GetNumInactiveStations.
6. Implement GetTotalProcessingTime (aggregate first; optionally introduce cached total later).
7. (Optional) Introduce cached active processing total and adjust state transitions.
8. Add XML docs to all public members; ensure exceptions documented.
9. Run tests; list and fix any failing scenarios.
10. Review invariants and edge cases; confirm they hold (inactive count = remaining unused capacity, NOT added-but-inactive stations).
11. Final cleanup: remove unused usings, ensure guard clause consistency, verify no dead code (note: adding beyond capacity should be guarded even though tests may not cover it).

### Acceptance
- All tests green.
- Guards & exception types match spec.
- Only the required public surface.
- Clear, intention‑revealing code.

See specs.md for: detailed rules, error handling, invariants, optional extensions, and pitfalls.





