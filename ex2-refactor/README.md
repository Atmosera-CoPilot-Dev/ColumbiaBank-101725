# Average Salary Refactor – Guided Prompts

This exercise starts with a deliberately naive and poorly implemented (Program.cs). Refactor it through the ordered steps in sequence only.

Tip: Add specs.md to the chat context BEFORE issuing a prompt from the ladder so Copilot honors all constraints

---

## Step 0 – Build & Clear Warnings (Prep)
Goal: Quiet build so any new warning = new issue.

Quick Steps:
1. Run: dotnet build
2. Note any compiler warnings (e.g., CS8618).
3. Apply the smallest temporary nullable fix (required keyword OR = string.Empty). No full validation yet.
4. Rebuild; ensure warnings gone (or consciously deferred).
5. Commit: "step 00 – baseline warnings addressed"

Prompt:
Identify all current compiler warnings (focus CS8618). Recommend the minimal change to silence them without full validation. Show ONLY the revised Employee definition.

Optional Follow-Up Prompt:
Confirm no remaining nullable warnings and how you verified this.

---

## Ordered Steps & Prompts

### Step 1 – Baseline Understanding
Prompt:
Explain what Program.cs currently does and list every hidden assumption or failure mode (null handling, negative salary, overflow, casing, empty department, no-match semantics). Include why CS8618 appeared and the intended long‑term validation approach.

### Step 2 – Risks & Assumptions Table
Prompt:
Provide a table (Risk | Cause | Impact | Mitigation) covering: monetary precision, overflow, negative salary, empty/blank department, casing differences, no-match handling, invalid entities, future extensibility.

### Step 3 – Architecture (Confirm Single Project)
Context: Keep a single project (plus existing test project). No multi‑project split.

Prompt 1:
Justify retaining a single project (benefits & acceptable trade‑offs).  
Prompt 2:
List the planned file layout (Domain/Employee.cs, Domain/InvalidSalaryException.cs, Domain/ISalaryCalculator.cs, Domain/SalaryCalculator.cs, minimal Program.cs) with namespace convention. (Planning only—no file moves yet.)

### Step 4 – Interface & Domain Sketch
Prompt 1:
Propose an initial Employee shape and a minimal ISalaryCalculator reflecting: decimal money, nullable decimal for no-match, OrdinalIgnoreCase department filter.  
Prompt 2:
Refine to remove premature abstractions—keep only what average calculation needs. Clarify the no‑match policy.

### Step 5 – Core Implementation (File Split + Guards)
Prompt:
Generate the actual files:
- Domain/Employee.cs (validation: Name trimmed & non-empty; salary >= 0 else InvalidSalaryException; department trimmed, then validated non-null/non-whitespace)
- Domain/InvalidSalaryException.cs
- Domain/ISalaryCalculator.cs (final refined version)
- Domain/SalaryCalculator.cs implementing:
  - decimal aggregation with checked arithmetic
  - nullable decimal no-match policy (return null when no employees match)
  - case-insensitive (OrdinalIgnoreCase) department filtering
  - fail-fast on any invalid Employee (e.g., negative salary)
  - explicit exception contract:
    * ArgumentNullException if employees is null
    * ArgumentException if department is null/whitespace
    * InvalidSalaryException if any employee has negative salary
    * OverflowException allowed to surface from checked sum
- Program.cs cleaned of inline type definitions (may remain minimal or empty)

Ensure: (a) Name & Department are trimmed before validation/filter, (b) no console/I/O in domain code, (c) null return for no matches (not 0), (d) negative salary throws InvalidSalaryException, (e) explicit exceptions as above, (f) checked arithmetic used, (g) no nullable warnings.

### Step 6 – XML Documentation
Prompt:
Add XML docs to all public members in the four domain files describing parameter constraints, each possible exception (ArgumentNullException, ArgumentException, InvalidSalaryException, OverflowException), and null return semantics (no matches). Show updated code only.

### Step 7 – Unit Tests (xUnit)
Prompt 1:
Create tests for:
- Multi-employee average
- Single employee average
- No matches => null (assert null explicitly)
- Negative salary creation throws InvalidSalaryException
- Empty / whitespace department rejected (Employee)
- Blank / whitespace Name rejected (Employee)
- Zero salary allowed (included in average)
- Large values precision maintained
- Case-insensitive positive match (e.g., "engineering" matches "Engineering")
- Null employees collection passed to calculator throws ArgumentNullException
- Null or whitespace department passed to calculator throws ArgumentException
Prompt 2:
Add tests for:
- All employees filtered out (case mismatch) => null
- (Optional) Overflow stress scenario (explain practicality limits for decimal if skipped)

### Step 8 – Alternative Implementation
Prompt:
Propose a streaming (single-pass) alternative calculator. Compare clarity, extensibility, memory, overflow risk with current implementation.

### Step 9 – Self-Critique
Prompt:
Critique the design: SOLID adherence, over-engineering risks, monetary precision/rounding, localization, validation boundaries, potential simplifications.

### Step 10 – Final Audit
Prompt:
Audit the solution verifying:
1. No System.Console or other I/O in Domain
2. checked arithmetic present in aggregation
3. Exception contract implemented (ArgumentNullException, ArgumentException, InvalidSalaryException, OverflowException)
4. All guardrails from specs adhered to
5. Tests cover name/department validation, null employees, case-insensitive positive & filtered-out scenarios, null no-match behavior
6. XML docs list all exceptions + null return meaning
7. No nullable warnings
List remaining risks/TODOs with status (Resolved / Acceptable / Action Needed / Deferred) and suggest only small, non-architectural improvements.

### Stretch – Async Adaptation (Optional)
Prompt:
Show adaptation to an async data source (IAsyncEnumerable<Employee>) via an AsyncSalaryCalculator variant; provide only new/changed files.

---

## Guardrails Summary (Excerpt of specs.md)
- decimal for money; average returns nullable decimal (null = no matches)
- Name trimmed & non-empty
- Department trimmed & non-empty; OrdinalIgnoreCase comparison
- Salary >= 0 (negative => InvalidSalaryException); zero allowed
- checked arithmetic (overflow surfaces)
- Domain code has no console/I/O/formatting
- Fail fast on invalid Employee
- Resolve nullable warnings early (Step 0)
- Each domain type in its own file (split performed in Step 5)
- specs.md not modified

---

## Completion Definition
Complete when:
- Steps executed in order
- Domain types separated (end of Step 5)
- No nullable or build warnings
- Tests cover: multi, single, no-match (null), filtered-out all, negative salary, zero salary, large precision, case-insensitive positive match, blank Name rejected, blank Department rejected, null employees arg throws, null/whitespace department arg throws
- Average no-match => null (never 0)
- Negative salary throws InvalidSalaryException
- Name & Department trimming & validation verified
- Domain layer free of Console / I/O usage
- checked arithmetic present in aggregation
- Full exception contract implemented & documented
- XML docs include parameter constraints, exceptions, null semantics
- No stray TODOs (unless explicitly deferred)

---

## Quick Run
dotnet test

---

## Troubleshooting Prompt
If guidance drifts:
"Re-align: decimal money, nullable decimal no-match, trimmed Name & Department, negative salary throws InvalidSalaryException, checked arithmetic, explicit exception contract, no domain I/O, single-project layout."





