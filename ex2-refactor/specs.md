# Average Salary Refactor – Specifications (Concise)

Purpose: Minimal, stable design contract. README = workflow; this file = authoritative rules.

---
## 0. Baseline Warning Policy
Resolve CS8618 early (string properties: required or = string.Empty). Later replaced by validated constructor.

### 0.1 Compiler Warnings (Captured)
(Record actual warning list here.)

### 0.2 Chosen Temporary Mitigation
(required / inline init / temporary ctor)

---
## 1. Core Design Decisions
- Money: decimal
- No-match: return null
- Salary: >= 0, negative => InvalidSalaryException, zero allowed
- Department: non-null/non-whitespace, OrdinalIgnoreCase
- Overflow: checked arithmetic
- Domain: no console / formatting
- Fail-fast if rogue invalid entity encountered
- Each domain type in its own file (no multi-type Program.cs after refactor)

---
## 2. Domain Model
Employee
- Name (trimmed, non-empty)
- Department (trimmed, non-empty, case-insensitive comparison)
- Salary (decimal >= 0)
Invariants enforced at creation.

ISalaryCalculator
- decimal? CalculateAverage(IEnumerable<Employee> employees, string department)
Throws: ArgumentNullException (employees), ArgumentException (department), InvalidSalaryException (negative salary), OverflowException (sum overflow). Returns null for no matches.

---
## 3. Exceptions
InvalidSalaryException (captures invalid value).

---
## 4. Algorithm Reference
1. Guard arguments
2. Filter (ignore case)
3. If none -> null
4. checked loop: validate non-negative, sum, count
5. return sum / count

---
## 5. Testing Targets (xUnit)
- Multiple average
- Single average
- No match => null
- Negative salary (creation)
- Null / whitespace department
- Null employees collection
- Zero salary included
- Case-insensitive match
- Large values precision
- (Optional) Overflow edge

---
## 6. XML Docs Must Cover
- Parameter constraints
- Exceptions
- Null return semantics

---
## 7. Stretch
Async adaptation via ISalarySource + CalculateAverageAsync (optional).

---
## 8. Change Log
Version | Description
------- | -----------
1.0.0 | Initial concise spec
1.0.1 | Added compiler warnings capture
1.0.2 | Added explicit file-per-type requirement
