## Byte Transformation with C# (Prompt Guide)

This exercise uses a small ByteFactory component to practice progressive, outcome‑oriented prompting with GitHub Copilot. The full design & testing specification has been moved to specs.md. 

Always add specs.md to the chat context BEFORE issuing a prompt from the ladder so Copilot honors all constraints (truncation, validity flag, non‑mutation, delegate behavior, etc.).

---
### Quick Objectives
- Encapsulate up to 1000 bytes with a validity flag.
- Provide non‑mutating XOR and delegate-based transformations.
- Enforce constructor validation & truncation rules.
- Drive implementation incrementally with tests (NUnit).

---
### Copilot Prompt Ladder
Follow these prompts in order. Stop after each, implement / run tests, then proceed. Keep specs.md in context.

1. High‑Level Intent (Existing Placeholder)
   Prompt: "Review the existing placeholder ByteFactory class and outline the target API (fields, constructor behavior, IsValid, GetByteCount, both Transform overloads) plus a test class outline. Do not implement yet—outline only."
2. Refine Data Constraints
   Prompt: "Add a size limit (10000 bytes) with a validity flag to the outline. Show constructor and IsValid method only (still outline-level)."
3. Implement Core Methods
   Prompt: "Implement GetByteCount and Transform(mask) using XOR; keep fields prefixed with m_. No tests yet."
4. Add Delegate Overload
   Prompt: "Add an overload Transform(Func<byte, byte> f) returning a new List<byte>; throw ArgumentNullException if f is null; do not mutate internal list."
5. Test Skeleton
   Prompt: "Create NUnit tests covering construction validity, empty list, over-limit list, Transform with mask examples, and delegate inversion example. Skeleton only, no assertions yet."
6. Add Assertions
   Prompt: "Fill in assertions for the test skeleton: verify counts, validity flag, XOR results, delegate inversion, and null delegate exception."
7. Edge Masks
   Prompt: "Add tests for mask 0x00 (identity) and 0xFF (bit inversion relative to XOR)."
8. XML Docs
   Prompt: "Add XML doc comments summarizing behaviors, especially handling of >1000 inputs and non-mutating transforms."
9. Streaming Extension (Optional)
   Prompt: "Propose a streaming TransformEnumerable(byte mask) plus one test."
10. Refactor & Review
    Prompt: "Review the ByteFactory implementation for clarity, immutability of returned results, and suggest micro refactors without changing behavior."
11. Performance Commentary (Optional)
    Prompt: "Estimate allocation differences between list-returning Transform and an iterator-based TransformEnumerable; add big-O comments."
12. Final Summary
    Prompt: "Summarize design choices (size guarding, immutability, delegate flexibility, truncation strategy) in 4 concise bullets for the README."

Tip: Keep each change minimal; run tests after every step. If Copilot drifts, restate key rules from specs.md.

---
### Minimal Testing Focus
Cover: constructor validation (null, empty, boundary, over-limit), truncation & IsValid flag, XOR correctness (sample + identity + inversion), delegate behavior (lambda inversion, wrap-around increment, null delegate), non-mutation (distinct list instances), determinism (repeatable outputs), boundary (1000 vs 1001). Optional: randomized parity check & streaming variant.

---
### Example Usage (After Core Implemented)
```csharp
var factory = new ByteFactory(new byte[] { 0x10, 0x20, 0x30 });
if (!factory.IsValid()) throw new InvalidOperationException();
var masked = factory.Transform(0x55);
var inverted = factory.Transform(b => (byte)~b);
```

---
### Remember
Add specs.md to Copilot context for every prompt to prevent requirement drift.






