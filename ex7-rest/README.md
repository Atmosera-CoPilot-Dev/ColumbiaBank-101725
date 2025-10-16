# Inventory Web API project with EF Core (SQLite) backend and MSTest integration tests.

## Overview
A minimal inventory Web API solution (C# / .NET 8) with three projects:
- DataLibrary: EF Core (SQLite) data layer (entities + DbContext).
- CoreWebApi: ASP.NET Core Web API (currently read-only Products endpoints).
- CoreWebAPI.Tests: MSTest integration tests (will expand).

Tip: Add specs.md to the chat context BEFORE issuing a prompt from the ladder so Copilot honors all constraints

## Copilot Workflow Prompts
Use the following ordered prompts (Ask each separately in Copilot Chat: Ask / Agent mode). Adapt naming as needed. Each step should build cleanly and keep tests green before moving on.

1. Understand the Codebase
Prompt: "Review the repository and summarize responsibilities & interactions among DataLibrary (entities, DbContext), CoreWebApi (Program, controllers), CoreWebAPI.Tests (test setup). List current endpoints, how the SQLite path is resolved, notable EF configuration, test strategy gaps, and potential risks. Provide a concise bullet summary plus clarification questions you would ask before implementing changes."

2. Initialize Migrations
Prompt: "Add EF Core migrations support to DataLibrary. Create an InitialCreate migration from the existing model, configure SQLite connection already present. Add a README snippet with command examples in a new docs/migrations.md file (do not alter main README). Apply migrations automatically at startup in Development only."

3. Align Nullability
Prompt: "Review schema vs entities and adjust property nullability or add a migration enforcing NOT NULL per rules in specs.md section 2 & 4. Produce migration 'AdjustNullability'. Update entities with XML docs."

4. Introduce DTOs & Mapping
Prompt: "Add DTO classes for Product, Category, Supplier (Create, Update, Read variants) plus summary DTOs. Add AutoMapper with profiles mapping entities to DTOs. Register AutoMapper in Program.cs. Refactor existing GET endpoints to return ProductDto with nested summaries. No breaking changes to route URLs."

5. Product CRUD Endpoints
Prompt: "Implement POST, PUT, PATCH, DELETE for Products using DTOs. Include validation attributes based on specs.md 4.4. Return appropriate status codes (201 with Location for create, 204 for successful delete). Add concurrency token RowVersion (byte[]) to Product and include in DTOs. Include optimistic concurrency handling returning 409 ProblemDetails with guidance. Update migrations."

6. Global Error Handling
Prompt: "Add middleware for global exception handling returning RFC 7807 ProblemDetails. Convert existing 404 and validation errors to ProblemDetails. Include correlation id (Guid) header 'X-Correlation-ID'. Integrate Serilog with console sink and enrichers (CorrelationId, RequestId)."

7. Category & Supplier CRUD
Prompt: "Add full CRUD endpoints for Category and Supplier using DTO pattern. Enforce cascade behavior per specs. Include concurrency with RowVersion. Add integration tests for typical and edge cases."

8. product_list View Endpoint
Prompt: "Map the product_list database view to a keyless entity ProductListItem. Add GET /api/Inventory/ProductList returning a collection of view DTO objects (flat structure). Use AsNoTracking. Add tests."

9. Query Enhancements (Products List)
Prompt: "Extend GET Products with pagination (pageNumber,pageSize), filtering (nameContains, categoryId, supplierId, minPrice, maxPrice) and sorting (name|price|stock asc/desc). Return pagination metadata headers (X-Pagination-TotalCount, X-Pagination-PageSize, X-Pagination-PageNumber, X-Pagination-TotalPages). Add tests."

10. Concurrency Tests
Prompt: "Add integration tests that simulate optimistic concurrency conflict on Product update (RowVersion mismatch) asserting 409 ProblemDetails payload schema. Add helper for creating conflicting updates." 

11. Refactor Test Infrastructure
Prompt: "Introduce a CustomWebApplicationFactory that centralizes seeding and database setup (use SQLite in-memory with migrations). Refactor existing tests to use it. Provide seeding utilities and ensure disposal cleanup."

12. Logging & Diagnostics
Prompt: "Enhance Serilog config with minimum levels (Information default, override Microsoft to Warning) and enrich logs with elapsed request timing. Add request logging middleware capturing method, path, status, duration ms."

13. CI Pipeline
Prompt: "Add GitHub Actions workflow .github/workflows/ci.yml performing: checkout, setup .NET 8, restore, build, test with coverage (coverlet), upload coverage artifact, run dotnet format check. Fail on formatting issues."

14. OpenAPI & Client Generation
Prompt: "Configure Swashbuckle for all environments except Production. Add NSwag or Swashbuckle CLI generation script to produce a TypeScript client into a new Client directory. Include npm package.json if needed. Provide docs/client.md usage instructions."

15. Hardening (Optional)
Prompt: "Add rate limiting (ASP.NET built-in) and security headers (basic middleware). Prepare for future JWT authentication (scaffold authentication services but leave disabled behind feature flag)."

16. Final Cleanup
Prompt: "Run code analyzers, ensure XML docs for public members, remove unused usings, add summary comments to new DTOs and controllers, update specs.md with final implemented sections and mark completed tasks." 

# Quick Commands
- dotnet build
- dotnet test
- dotnet run --project CoreWebApi

# License
Internal training project. Add LICENSE if distributing.