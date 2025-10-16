================================================================================
# Exercise: Bank Transaction Monitoring – Prompt Ladder & Guide

Brief overview: Implement an in‑memory .NET 8 banking core with account rule enforcement (checking, savings, business), atomic deposit/withdraw/transfer, immutable ledger + audit trail, pluggable suspicious activity rules (large amount, rapid fire) with idempotent review, and a daily summary—driven by lean stepwise prompts and fast unit tests.

---
## Goal Snapshot
Build a .NET 8 core that fulfills all requirements for account rules, atomic transactions, immutable ledger and audit, suspicion rule evaluation, idempotent review, daily summary, and fast unit test coverage.

---
## Suggested First Prompt

"I am an experienced software engineer and would like an overview of how GitHub Copilot can help me deliver a robust solution for the Bank Transaction Monitoring System specification.

- Explain how Copilot can assist in the design, implementation, testing, deployment, and maintenance phases, given that the requirements and constraints are already defined.
- Provide specific examples of Copilot’s capabilities for generating code, enforcing domain rules, writing unit tests, supporting concurrency and audit logic, and maintaining extensibility.
- Reference the specification to illustrate how Copilot can help address technical challenges and ensure compliance with acceptance criteria."

Tip: Add specs.md to the chat context BEFORE issuing a prompt from the ladder so Copilot honors all constraints
