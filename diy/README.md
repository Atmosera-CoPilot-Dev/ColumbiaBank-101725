# DIY Workshop Exercises

This folder contains several Do It Yourself (DIY) C# exercises. Each problem has its own scenario specification file inside its folder. Read the scenario first, then implement from scratch following its rules.

## 1. Healthcare Mini-Normalizer (healthcare/healthcare-scenario.md)
Stream a single mixed VISIT/LAB CSV, validate rows, normalize into minimal domain objects, map simple codes, mask names, and output summary metrics. Focus on layered validation, pluggable terminology mapping, and privacy via masking. Goal: lean console app + ~5–6 NUnit tests.

## 2. Bank Transaction Monitoring System (banking/bank-scenario.md)
Implement an in-memory transactional core handling deposits, withdrawals, transfers with thread safety and immutable ledger. Add pluggable suspicious activity rules, auditing, daily summary reporting, and asynchronous APIs. Emphasis: correctness, concurrency invariants, and comprehensive fast tests.

## 3. Manufacturing Simulation (manufacturing/microchips.md spec; see manufacturing/README-chips-scenario.md)
Simulate daily widget production lines plus quality control flow with deterministic seeded randomness. Track production, damage, failures, inspections, defects, backlog, utilization, and efficiency across a minute-based loop. Deliver a console runner and NUnit tests confirming determinism and key metrics.

---

