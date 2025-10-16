# ByteFactory Specification

This document contains the detailed design & testing specification for the ByteFactory exercise. Use it as authoritative context when working through the ordered prompts in README.md. Always add this file to Copilot Chat context before asking for code generation so the assistant honors all constraints.

---
## 1. Objectives
1. Clean class design & encapsulation for low?level data handling.
2. Safe handling of collection constraints (size limit & validity flag).
3. Method overloading + delegate / lambda extensibility.
4. TDD with incremental, outcome?oriented prompts.
5. Progressive prompt refinement (“prompt ladder”).
6. Reflection on code quality, readability, and extensibility.

---
## 2. Functional Specification
Class: `ByteFactory`

Requirements:
- Internal storage: private `List<byte> m_bytes` initialized from constructor input (any `IEnumerable<byte>`). Make a defensive copy.
- Capacity rule: maximum count = 1000. If source length exceeds this limit, mark internal validity flag `m_isValid = false`.
- Behavior on overflow (Option A – expected): store only the first 1000 bytes, discard the remainder, flag invalid.
- Expose `bool IsValid()` returning the validity flag.
- Expose `int GetByteCount()` returning the stored count (even if invalid).
- Provide `List<byte> Transform(byte mask)` returning a new list where each element = `original ^ mask`.
- Provide `List<byte> Transform(Func<byte, byte> transformFunc)` returning a new list where each element = result of delegate; throw `ArgumentNullException` if delegate is null.
- Never mutate internal list in any `Transform` method; always allocate a fresh list.
- Deterministic & side?effect free except constructor validation.

### Edge / Behavior Notes
- Null source: throw `ArgumentNullException`.
- Over limit input: truncate to 1000 (Option A) + invalidate flag.
- Empty input: valid; transforms return empty list (not null).
- Delegate variant must preserve order.
- No in?place mutation unless added as explicit extension.

---
## 3. Suggested Extension Ideas (Optional After Core)
- `TransformInPlace(byte mask)` mutating variant (compare performance & memory).
- Streaming: `IEnumerable<byte> TransformEnumerable(byte mask)` using `yield return`.
- Pipeline: accept `IEnumerable<Func<byte, byte>>` and apply sequentially.
- Metrics: count transforms performed (thread?safe increment).

---
## 4. Testing Strategy (Detailed)
Framework: NUnit.

### Categories & Representative Cases
1. Construction & Validation
   - Null input => `ArgumentNullException`.
   - Valid input within limit => `IsValid == true`.
   - Over limit (1001+) => `IsValid == false`; stored count == 1000 (truncated) and no element beyond index 999.
   - Empty input => valid, count 0.
2. Counting & Internal State
   - `GetByteCount` correct for empty, boundary (1000), over-limit.
3. XOR Transform Behavior
   - Single value: `[0x99]` with mask `0x55` => `[0xCC]`.
   - Identity mask `0x00` => deep-equal bytes (different list instance).
   - Inversion mask `0xFF` => per-byte bitwise NOT.
4. Delegate Transform
   - Inversion lambda `(byte b) => (byte)~b` matches XOR 0xFF result for sample set.
   - Increment lambda wrap: 0xFF -> 0x00.
   - Null delegate => `ArgumentNullException` (optional assert ParamName).
5. Empty Input Invariance
   - Both transforms return empty list not null.
6. Non-Mutation & Idempotence
   - Multiple `Transform(mask)` calls produce new instances; original bytes intact.
7. Boundary Conditions
   - Exactly 1000 bytes valid.
   - 1001 bytes invalid, truncated to 1000; transform processes only stored subset.
8. Determinism & Purity
   - Repeated identical calls produce value-equal outputs (distinct references).
9. Randomized Parity (optional)
   - Fixed seed random array (128 bytes) verify XOR 0xFF vs delegate inversion parity.

### Additional Quality Techniques
- Parameterized tests with `[TestCase]` or `[TestCaseSource]` for multiple mask scenarios.
- Negative path tests early to ensure fail-fast.
- Ensure reference inequality when expecting new list allocations.

### Naming Guidelines
Pattern: `MethodOrScenario_Condition_ExpectedOutcome` (e.g., `Transform_WithMask55_OnSingleByte99_ReturnsCC`). Use uppercase hex in names.

### Coverage Goal
Target 100% of public methods & branching (valid / invalid constructor path, delegate null path).

---
## 5. Suggested Testing Prompts (Verbatim)
(Use progressively; keep `specs.md` open in context):
- Foundational Skeleton: "Generate an NUnit test class ByteFactoryTests with setup showing valid, empty, over-limit construction. Only method stubs, no assertions." 
- Constructor Edge Cases: "Add NUnit tests verifying ByteFactory throws ArgumentNullException on null source and truncates over-limit input to 1000 while setting IsValid false." 
- Counting & State: "Write NUnit tests for GetByteCount covering empty, exactly 1000, and over-limit (1001) inputs. Include assertions for IsValid flag." 
- XOR Transform Cases: "Add tests verifying Transform(mask) with inputs [0x99] mask 0x55 => 0xCC, identity mask 0x00 returns original bytes, and mask 0xFF equals bitwise NOT of each byte." 
- Delegate Overload: "Create tests for Transform(Func<byte, byte>) including inversion lambda, increment with wrap, and null delegate throwing ArgumentNullException." 
- Parameterized Masks: "Introduce NUnit TestCase attributes to cover multiple (inputSequence, mask, expectedHexSequence) scenarios for Transform(byte mask)." 
- Non-Mutation Guarantee: "Add a test proving that repeated Transform calls return new list instances and original internal bytes are unchanged." 
- Randomized Cross-Check: "Generate a reproducible test (fixed Random seed) comparing results of Transform(0xFF) and Transform(b => (byte)~b) for 128 random bytes." 
- Boundary Conditions: "Add tests for exactly 1000 bytes (valid) and 1001 bytes (invalid) ensuring stored count is 1000 and transforms operate on truncated list." 
- Documentation Enforcement: "Add XML doc comments to ByteFactory summarizing size limit, truncation behavior, and non-mutation; do not change implementation." 
- Coverage Gap Audit: "List any untested branches or exception paths in ByteFactory and propose new NUnit tests to cover them." 
- Performance Exploration: "Generate a benchmark-style test (not strict perf) comparing Transform(byte) vs. TransformEnumerable(byte) over 1000 bytes counting allocations conceptually in comments." 
- Final Review: "Review all ByteFactory tests for redundancy and propose one consolidation using parameterized cases without reducing clarity." 

---
## 6. Example Usage
```csharp
var factory = new ByteFactory(new byte[] { 0x10, 0x20, 0x30 });
if (!factory.IsValid()) throw new InvalidOperationException();

List<byte> masked = factory.Transform(0x55);           // XOR each
List<byte> inverted = factory.Transform(b => (byte)~b); // Custom delegate
```

---
## 7. Success Criteria (Acceptance Snapshot)
- Constructor enforces null & size rules.
- Truncation + invalid flag for >1000 inputs.
- All transforms non-mutating.
- Delegate overload null-guarded.
- Tests cover core + negative paths.
- XML docs describe truncation & immutability.

---
## 8. Extension Acceptance (Optional)
If extensions implemented, document:
- In-place vs non-mutating performance observation.
- Streaming method complexity & allocation commentary.
- Metrics counter thread-safety approach.

---
## 9. Notes for Copilot Usage
Always add BOTH README.md and specs.md to the chat context before issuing prompts from the ladder to avoid drift (e.g., forgetting truncation rule or non-mutation guarantee).

---
Happy building! Keep diffs small and verify tests after each ladder step.
