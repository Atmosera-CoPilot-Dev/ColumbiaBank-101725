# Bank Transaction Monitoring System – Specification
---

## 1. Scope

Deliver an in-memory transaction monitoring core that:
- Manages multiple account types with distinct balance rules.
- Processes deposits, withdrawals, and transfers atomically and thread-safely.
- Records an immutable transaction ledger.
- Detects suspicious activity via pluggable rules.
- Produces a minimal daily summary and maintains an audit trail.
- Is fully covered by fast NUnit tests (<5s).

---

## 2. Domain Model Requirements

### Accounts
- Abstract BankAccount (Id Guid, AccountNumber long, AccountType enum, Balance decimal).
- Derived types:
  - Checking: no overdraft (reject if withdrawal/transfer exceeds balance).
  - Savings: enforce configured minimum balance (post?operation balance must be >= minimum).
  - Business: allow overdraft up to configured negative limit (Balance >= -OverdraftLimit).
- Thread-safe balance mutation (must not produce torn reads or lost updates).
- Unique account numbers (monotonically increasing or GUID surrogate + human-friendly counter).
- All monetary amounts: decimal; reject negatives.

### Transactions
- Transaction fields: Id (Guid), UtcTimestamp (UTC), Type (Deposit | Withdrawal | Transfer), Amount, SourceAccountId, TargetAccountId (nullable), Status (Succeeded | Rejected), FailureReason (nullable).
- Ledger: append-only, immutable once recorded. Exposed via snapshot (copy) only.
- Transfers debit source and credit target atomically (all-or-reject). No partial states.

### Suspicious Activity
- ISuspiciousRule: Id, Description, Evaluate(tx, recentTxForSameSourceOrGlobal) -> bool/record trigger.
- Rules (baseline):
  1. LargeAmountRule: fires when tx.Amount > configured threshold.
  2. RapidFireRule: fires when count of transactions from same source within rolling window >= configured threshold.
- Detector:
  - Evaluates each successful transaction.
  - Emits SuspicionRecord (TransactionId, RuleId, CreatedUtc, Severity, ReviewedUtc?, Analyst?, ResolutionNote?).
  - Supports querying open/all and marking records reviewed (idempotent).

### Reporting & Audit
- Daily summary generation (per UTC date boundary):
  - TotalTransactions
  - SucceededCount
  - RejectedCount
  - SuspiciousCount (open or total? — choose and document; default: total created that day)
  - Net balance deltas per AccountType (sum of all applied deltas).
- Audit entries (append-only):
  - Fields: UtcTime, Actor (system if internal), Action, EntityType, EntityId, Metadata (key/value or serialized).
  - Actions to log at minimum: AccountCreated, AccountClosed (if implemented), TransactionProcessed, SuspicionRaised, SuspicionReviewed.

---

## 3. Functional Requirements

1. Create accounts asynchronously; retrieval by Id.
2. DepositAsync:
   - Reject zero or negative amount.
   - Succeeds if applied; logs transaction + audit.
3. WithdrawAsync:
   - Reject zero/negative amount.
   - Enforce account-specific constraints (no overdraft / min balance / overdraft window).
4. TransferAsync:
   - Reject zero/negative amount.
   - Enforce constraints on source as withdrawal would.
   - Apply atomically: either both sides recorded (one Transaction) or rejected.
5. Suspicious evaluation runs after a transaction is marked Succeeded (not for Rejected).
6. MarkSuspicionReviewedAsync must set review metadata exactly once (subsequent calls no-op or reject — document behavior).
7. All public APIs Task-based and non-blocking (no sync over async).

---

## 4. Non-Functional Requirements

- Concurrency:
  - No data races under high parallel mixed operations.
  - Deterministic lock ordering for multi-account operations (e.g., order by Guid) to prevent deadlocks.
  - Invariant: Sum of all balances after any set of successful transfers equals initial sum plus net deposits minus withdrawals.
- Time:
  - All timestamps use DateTime.UtcNow.
- Configuration:
  - Central MonitoringSettings (LargeAmountThreshold, RapidFireCount, RapidFireWindow, BusinessOverdraftLimit, SavingsMinimumBalance).
- Security / Safety:
  - Input validation (throw for invalid arguments before side effects).
  - No PII stored or logged.
  - Audit and suspicion records omit sensitive data (only IDs and technical metadata).
- Testability:
  - Deterministic logic; no randomization or wall-clock coupling beyond UTC retrieval (optionally abstractable).
- Extensibility:
  - Additional rules pluggable without modifying core transaction processor (open-closed).
  - Potential future persistence layer can replace in-memory repositories through interfaces.

---

## 5. Acceptance Criteria

- All public operations are asynchronous (Task-returning, no blocking waits).
- Savings withdrawal below configured minimum is rejected deterministically.
- Business overdraft allowed only up to configured negative limit; exceeding attempt rejected.
- Checking overdraft always rejected.
- Transfer operations preserve account sum invariant.
- Suspicious rules fire only under documented thresholds (strict > for LargeAmount; >= count for RapidFire).
- All timestamps UTC.
- MonitoringSettings centralizes all thresholds/limits.
- No lost or duplicated ledger entries; each API call produces at most one Transaction record.
- Daily summary aggregates counts and per-account-type net deltas accurately for the specified date.
- Audit log contains required actions with correct correlations.
- Tests (NUnit) complete in under 5 seconds and cover:
  - Account rule enforcement
  - Concurrency invariant (parallel transfers)
  - Suspicious rule trigger + non-trigger cases
  - Reporting aggregation accuracy
- Public types and members have XML documentation comments.

---

## 6. Out of Scope (Baseline)
- Persistent storage (database, files)
- Authentication / authorization
- Currency conversions / multi-currency handling
- Interest accrual or fees
- External logging infrastructure
- Cancellation tokens (optional stretch)
- UI / API endpoints

---

## 7. Glossary
- Overdraft Limit: Absolute magnitude of allowed negative balance for Business accounts.
- Minimum Balance: Lowest permissible post-operation balance for Savings accounts.
- Net Balance Delta (per account type): Sum of all successful transaction deltas restricted to accounts of that type for the reporting window.

---

## 8. References
- Implementation guidance & ordered prompts: see README.md in banking folder
- Testing conventions: NUnit (Test project separate)

---
