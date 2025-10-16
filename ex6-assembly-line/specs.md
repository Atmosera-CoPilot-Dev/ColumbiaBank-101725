# FactoryAssemblyLine Detailed Specification (Aligned With Tests)

This document contains the detailed design, behavioral rules, and optional extensions. It is aligned with the current NUnit tests in `tests/FactoryAssemblyLineTests.cs`.

## Domain Model
Station:
- Id (int > 0, unique among added stations)
- ProcessingTime (int > 0)
- IsActive (bool)

All state is in-memory. No threading requirements. No persistence or async.

## Data Structure
`Dictionary<int, Station>` keyed by stationId.
`Station` may be a private mutable class or record; choose clarity.

## Public API (from IFactoryAssemblyLine)
- AddStation(int stationId, int processingTime)
- RemoveStation(int stationId)
- StartAssembly(int stationId)
- StopAssembly(int stationId)
- GetProcessingTime(int stationId)
- GetTotalProcessingTime()
- GetNumStations()
- GetNumActiveStations()
- GetNumInactiveStations()
- IsStationActive(int stationId)

## Capacity & Counting Semantics (Test-Driven)
Constructor parameter `numStations` represents the fixed capacity of the assembly line.
- `GetNumStations()` returns this capacity (NOT the number of currently added stations).
- Added (but not yet started) stations DO NOT count as inactive stations. Inactive count reflects remaining unused capacity slots.
  * Inactive slots = capacity - addedStationCount.
- Active stations are those added AND started.
- Removing a station frees a capacity slot (inactive count increases by 1; active count decreases if it was active).

Summary formulas:
- activeCount = number of stations where IsActive == true.
- inactiveSlots = capacity - addedStationsCount.

## Validation & Exceptions (Aligned With Tests)
Throw:
- `ArgumentException` when:
  - Constructor `numStations` < 0
  - `stationId <= 0` (any API receiving a stationId)
  - `processingTime <= 0` in AddStation
  - Adding a duplicate stationId (duplicate is treated as an argument problem)
  - (Optionally) attempting to add more stations than capacity (test suite does not cover; still reasonable guard)
- `ArgumentOutOfRangeException` when:
  - Operating on a stationId that does not exist (Remove/Start/Stop/GetProcessingTime/IsStationActive)

Do NOT leak KeyNotFoundException.

## Idempotency Policy
- StartAssembly on an already active station: no-op.
- StopAssembly on an already inactive station: no-op.
(No tests assert exceptions for repeat calls.)

## Complexity Targets
All operations O(1) except `GetTotalProcessingTime()` which may be O(n) (where n = active stations or total added stations). Optimization (cached total) optional.

## Optional Cached Total
Maintain an `_activeProcessingTotal` updated on state changes:
- Activate: +processingTime
- Deactivate: -processingTime
- Remove active: -processingTime
- Add inactive: no change
Invariant (debug): `_activeProcessingTotal == Sum(active stations processingTime)`.

## Invariants
1. 0 <= addedStationsCount <= capacity.
2. Active stations subset of added stations.
3. activeCount <= addedStationsCount.
4. inactiveSlots = capacity - addedStationsCount.
5. activeCount + inactiveSlots <= capacity (strictly < unless all added stations are active).
6. All processing times > 0.

## Edge Cases
- Add duplicate stationId -> ArgumentException.
- Start non-existent -> ArgumentOutOfRangeException.
- Stop non-existent -> ArgumentOutOfRangeException.
- Remove non-existent -> ArgumentOutOfRangeException.
- GetProcessingTime non-existent -> ArgumentOutOfRangeException.
- IsStationActive non-existent -> ArgumentOutOfRangeException.
- Repeated Start/Stop: no state change, no exception.

## Implementation Guidance
Use guard clauses. Keep logic minimal. Prefer early validation. Avoid recalculating counts each call; maintain simple counters:
- `_addedStationsCount`
- `_activeStationsCount`
Optionally `_activeProcessingTotal` if caching.

## Method Behaviors
- AddStation: validate arguments, ensure capacity not exceeded & uniqueness, add inactive station, increment `_addedStationsCount`.
- RemoveStation: validate existence; if active decrease `_activeStationsCount`; remove entry; decrement `_addedStationsCount`.
- StartAssembly: validate existence; if inactive set active and increment `_activeStationsCount`.
- StopAssembly: validate existence; if active set inactive and decrement `_activeStationsCount`.
- GetProcessingTime: validate existence; return stored value.
- GetTotalProcessingTime: sum active stations (or return cached total).
- GetNumStations: return capacity.
- GetNumActiveStations: return `_activeStationsCount`.
- GetNumInactiveStations: return `capacity - _addedStationsCount`.
- IsStationActive: validate existence; return IsActive.

## Optional Extensions (Not Required)
- UpdateProcessingTime(stationId, newTime > 0) with cache adjustment.
- Thread safety via a private lock.
- Enumeration APIs for active station ids.

## Pitfalls to Avoid
- Misinterpreting inactive count as (added - active) – tests expect unused capacity.
- Wrong exception types (use ArgumentOutOfRangeException for missing stations).
- Forgetting to adjust active count on removal or stop.
- Double increment/decrement on idempotent Start/Stop.

## Readiness Checklist
- All interface methods implemented.
- Counts & exceptions conform to tests.
- Idempotent Start/Stop.
- No KeyNotFoundException leaks.
- Tests pass (green).
- Code clear & lean.
