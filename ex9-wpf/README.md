# Exercise 9 – WPF Address Book Refactor & Enhancement

Goal: Use GitHub Copilot (chat + inline) to understand, refactor, and enhance the WPF MVVM sample for robustness, testability, clarity, maintainability, and selected feature improvements.

Tip: Add specs.md to the chat context BEFORE issuing a prompt from the ladder so Copilot honors all constraints.

## Overview
This exercise focuses on disciplined, incremental refactoring driven by explicit design and quality goals. You will:
- Assess the current architecture and risks
- Define a refactor and enhancement plan (repository abstraction, validation, async, commands, dirty tracking, tests)
- Implement improvements iteratively with small, reviewable changes
- Use structured prompting (ask, review, refine) rather than accepting first suggestions

All detailed specifications, constraints, quality checklist, and extended guidance have been moved to specs.md. Load that file in Copilot Chat before using the prompts below.

##  Prompt Ladder
Treat the following as a reusable ladder of targeted prompts to progress through the exercise. Reference specs.md for the authoritative requirements.

### Baseline Understanding
1. "Summarize the overall architecture of this WPF app (patterns, layers, responsibilities)."
2. "Explain MainViewModel.cs line by line; list assumptions and potential failure modes."
3. "Explain how ContactCardManager.Load works and what happens if the XML file is missing or malformed."
4. "List all public properties that lack validation and potential user-facing issues."
5. "Identify all uses of magic strings and propose safer alternatives."
6. "List threading or async concerns in current load/save implementation."
7. "Explain the lifetime/Dispose pattern used and whether it is idiomatic for this scenario."

### Risk Expansion & Planning
8. "Expand the current risk list with additional maintainability and UX concerns."
9. "Propose a refactor plan grouped into Safety, Reliability, UX, Maintainability."
10. "Suggest an interface IContactRepository with async LoadAsync/SaveAsync and exception contract."
11. "Design a minimal validation strategy (DataAnnotations or custom) for ContactCard and ContactAddress."
12. "Propose a dirty tracking approach for contacts (interface or change flag)."
13. "Suggest how to integrate INotifyDataErrorInfo for per-field validation in WPF."

### Implementation Iteration
14. "Refactor MainViewModel to use an injected IContactRepository; create XmlContactRepository (async)."
15. "Replace property change magic strings with nameof usage across ViewModels."
16. "Add explicit SaveCommand and dirty tracking; prompt only when dirty on close."
17. "Implement DataAnnotations and INotifyDataErrorInfo for validation feedback."
18. "Add CanExecute logic updates for RemoveCommand when SelectedCard changes." 
19. "Make load/save fully async without blocking UI; introduce IsBusy flag." 
20. "Introduce unit tests for repository, validation, dirty tracking, and add/remove behaviors (NUnit)."
21. "Polish: trim/normalize input fields, centralize constants, add XML docs to public members."

### Extended Enhancements (Optional)
22. "Suggest XAML DataTemplate with validation highlighting for a contact."
23. "Add a toolbar with Add / Remove / Save commands bound appropriately."
24. "Implement a search TextBox filtering the Contacts collection (case-insensitive)."
25. "Implement JsonContactRepository parallel to XmlContactRepository behind same interface."
26. "Integrate CollectionViewSource for sorting and filtering by search text."
27. "Add tests for validation edge cases (empty, whitespace, invalid email/zip)."

### Quality & Wrap-Up
28. "Audit the solution against the quality checklist (from specs.md) and list gaps."
29. "Generate a final audit report with remediation steps for any remaining gaps."
30. "Summarize all refactors applied, rationale, and recommended future improvements (FINAL_NOTES.md)."


Good luck—focus on clarity, safe evolution, and disciplined iterative improvement. Remember to include specs.md in chat context before using these prompts.
