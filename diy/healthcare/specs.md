# Scenario: Healthcare Mini-Normalizer Specification

## Objective
Design and implement a focused C# .NET 8 console application that ingests a single CSV (mixed VISIT + LAB rows), produces canonical domain objects, performs layered validation + terminology mapping, applies simple name masking for privacy, tracks core metrics, and prints a concise summary. Provide a lean test suite (5–6 tests). Single-process, in?memory, single file input—no databases.

Target: ~90 minutes for an experienced engineer leveraging Copilot effectively.

## Scope Overview 
- Streaming line processing (no full file load)
- Layered validation (syntax ? semantic) with categorized rejection reasons
- Name privacy via masking only (hashing removed for simplicity)
- Small configuration object (file path + thresholds only)
- Pluggable terminology mapper (interface + default in-memory implementation)
- Minimal structured logging (info / warn / error)
- Basic large file guardrails (soft warning + hard stop)
- Validation rules: structural, MRN presence/format, date parsing, future date, lab numeric, name length
- Core metrics only (no timing or memory sampling)
- Tests focus on rejection rules, mapping, masking, duplicates, unmapped counting
- Excluded: hashing, advanced runtime metrics, memory sampling, detailed raw line storage

## Core Data Models (DO NOT ADD EXTRAS)
- Patient: MRN, Name, MaskedName, DOB
- Visit: VisitId (GUID), MRN, VisitDate, ReasonCodeOriginal, ReasonCodeMapped
- LabResult: ResultId (GUID), MRN, TestCodeOriginal, TestCodeMapped, Value (decimal?), CollectedDate
- Rejection: LineNumber, ReasonCode
- Metrics (container): totalLines, totalPatients, visitCount, labCount, rejectedCount, unmappedCodesCount

Keep models minimal—no extra diagnostic fields.

## File Format
`RecordType,MRN,PatientName,DOB,Date,Code,Value`

Types:
- VISIT: Date = VisitDate, Code = ReasonCode, Value ignored
- LAB: Date = CollectedDate, Code = TestCode, Value = numeric lab value

## Validation (Layered)
1. Structural: correct column count (7) ? reject `Structural`
2. Missing MRN (after trim) ? `MissingMRN`
3. MRN format invalid (`[A-Za-z0-9_-]{1,32}`) ? `InvalidMRNFormat`
4. Name length >128 ? `NameTooLong`
5. Date parse failure (DOB or event date) ? `InvalidDate`
6. Future DOB or event date (> UtcNow.Date) ? `FutureDate`
7. RecordType not VISIT/LAB ? `UnknownRecordType`
8. LAB value present but non-numeric ? `InvalidLabValue`
9. Optional warning: empty Code (if implemented) ? `WarnEmptyCode` (not a rejection)

Each rejection produces one Rejection entry and increments `rejectedCount`.

## Terminology Mapping (Pluggable)
```
public interface ITerminologyMapper {
  string MapReason(string original, Action unmappedCallback);
  string MapTest(string original, Action unmappedCallback);
}
```
Defaults: Reason R001?Annual, R002?FollowUp else "Unknown" (counts unmapped). Tests GLU?LOINC-GLU, HCT?LOINC-HCT else unmapped (empty mapped string, counts unmapped).

## Privacy
- Masking only: first character (if letter/digit) + `***`; else `*` for empty/null.
- No hashing retained—reduces complexity.

## Configuration Object
```
public class NormalizerConfig {
  public string DataFilePath { get; init; }
  public int SoftLineWarningThreshold { get; init; } // e.g. 100_000
  public int HardLineStopThreshold   { get; init; } // e.g. 1_000_000
}
```

## Processing Flow (Streaming)
1. Enumerate `File.ReadLines`
2. Increment `totalLines`
3. Parse ? DTO
4. Validate ? on failure record Rejection & continue
5. Upsert Patient (by trimmed MRN) with masking
6. Map codes (unmapped callback increments `unmappedCodesCount`)
7. Create Visit or LabResult; increment respective metric
8. End: print summary + patient list + optional grouped rejections

## Metrics Summary Line
`Lines=X Patients=Y Visits=V Labs=L Rejected=R Unmapped=U`

## Logging (Minimal)
Interface:
```
public interface ILogger { void Info(string msg); void Warn(string msg); void Error(string msg, Exception? ex = null); }
```
Log startup, soft threshold reached, first few rejections (optional), summary.

## Error Resilience
- Invalid lines ? rejection (no crash)
- Missing file ? clear error + non-zero exit

## Tests (Target 5–6)
1. Missing MRN ? MissingMRN rejection
2. Unknown reason code ? mapped Unknown + unmapped increment
3. Unknown test code ? unmapped increment (empty mapped)
4. Masking edge cases (normal name, single char, empty)
5. Future date ? FutureDate rejection
6. Invalid lab numeric value ? InvalidLabValue (optional if time)
7. Duplicate MRN not double-counted (optional swap with 6)

## Time Allocation (Suggested)
- 10 min: Models + config + mapper/logger interfaces
- 15 min: Parser + validation
- 15 min: Mapping + patient registry + masking
- 15 min: Orchestrator + metrics + summary
- 20 min: Tests
- 5 min: Cleanup / README notes

## Acceptance Criteria
MUST:
- Print summary line with all six core metrics
- At least 3 distinct rejection reasons possible
- Unmapped reason & test both increment `unmappedCodesCount`
- MaskedName never equals original name
- Duplicate MRNs counted once
- Single pass streaming
- ?5 tests all pass

SHOULD:
- Pluggable mapper interface present
- Soft/hard thresholds honored

MAY:
- Warnings counter (if `WarnEmptyCode` implemented)
- Simple interactive loop (keep tiny)

## Non-Goals
- Hashing / cryptographic functions
- Runtime duration or memory sampling metrics
- Raw line storage in Rejection
- DI container / async / external packages

## Suggested Copilot Prompt Sequence
1. "Generate POCO classes for Patient, Visit, LabResult, Rejection, MetricsContainer." 
2. "Create ITerminologyMapper + default mapper implementation." 
3. "Write a ParseLine function returning a DTO or error enum." 
4. "Implement validation functions (structural, MRN, dates, future date, lab value)." 
5. "Implement masking utility and patient upsert by MRN." 
6. "Implement ProcessLines orchestrator updating metrics." 
7. "Generate NUnit tests for missing MRN, unknown codes, masking, future date, invalid lab value." 

## Reflection Prompts
- Which validation rule caught the most test cases?
- What would you add first if hashing were reintroduced?
- How would you extend to support a second file type?

---
Focused scope: normalization, validation, mapping, masking, core metrics—nothing more.
