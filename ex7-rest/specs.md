# Design Specifications

## 1. Solution Structure
Projects (all target .NET 8):
- DataLibrary: EF Core data access layer (SQLite). Contains POCO entities, DbContext, fluent configuration, and (future) configurations for migrations & seeding.
- CoreWebApi: ASP.NET Core Web API exposing inventory data.
- CoreWebAPI.Tests: MSTest integration test project (will evolve to use a consistent CustomWebApplicationFactory and in?memory provider or SQLite in-memory mode).

## 2. Domain Model
Relational objects represented by three primary tables and one view.

### 2.1 Tables
- suppliers
  - SupplierId (INTEGER, PK)
  - SupplierName (TEXT, NOT NULL)
  - ContactName (TEXT, NULLABLE in SQL script)  (Decision: either mark entity property nullable or enforce NOT NULL via migration.)
  - Phone (TEXT, NOT NULL)
- categories
  - CategoryId (INTEGER, PK)
  - CategoryName (TEXT, NOT NULL)
  - SupplierId (INTEGER, FK ? suppliers.SupplierId, ON DELETE CASCADE)
- products
  - ProductId (INTEGER, PK)
  - ProductName (TEXT, NOT NULL)
  - CategoryId (INTEGER, FK ? categories.CategoryId, ON DELETE CASCADE)
  - Price (REAL / NUMERIC, nullable in script?)
  - Stock (INTEGER, nullable in script?)

### 2.2 View
- product_list (combines product + category + supplier projection). Will be exposed via a keyless entity mapped with `modelBuilder.Entity<ProductListItem>().HasNoKey().ToView("product_list")`.

### 2.3 Entity Relationships
- Supplier 1..* Category (Cascade delete categories when supplier removed.)
- Category 1..* Product (Cascade delete products when category removed.)

### 2.4 Planned Concurrency
- Add `RowVersion` (byte[]) to each mutable root (Supplier, Category, Product) with `[Timestamp]` for optimistic concurrency.

## 3. Data Access Layer
`InventoryDbContext` responsibilities:
- Configure table & column names explicitly for clarity and future migrations.
- Apply required vs optional property nullability to align with chosen strategy (either adapt entities or add migrations altering schema to NOT NULL where business requires).
- Configure cascade behaviors explicitly to avoid implicit cascade pitfalls.
- Seed baseline data (move from raw SQL script to EF Core seeding or a runtime seeder service) to support automated test environments.
- Register keyless view entity for `product_list`.
- Future: apply value converters or precision for Price (e.g., decimal(18,2)).

## 4. API Layer
### 4.1 Current Endpoints
- GET /api/Inventory/Products ? Returns all products including Category + Supplier (eager loaded).
- GET /api/Inventory/Products/{id} ? Returns single product or 404 with message.

### 4.2 Planned Endpoints (CRUD)
Products
- POST /api/Inventory/Products
- PUT /api/Inventory/Products/{id}
- PATCH /api/Inventory/Products/{id}
- DELETE /api/Inventory/Products/{id}

Categories
- GET /api/Inventory/Categories
- GET /api/Inventory/Categories/{id}
- POST /api/Inventory/Categories
- PUT /api/Inventory/Categories/{id}
- PATCH /api/Inventory/Categories/{id}
- DELETE /api/Inventory/Categories/{id}

Suppliers (analogous CRUD)

View
- GET /api/Inventory/ProductList

### 4.3 DTO Strategy
Introduce read and write DTOs:
- ProductDto (read) with nested CategorySummaryDto and SupplierSummaryDto.
- ProductCreateDto / ProductUpdateDto (no IDs in create; only mutable fields in update; Price, Stock optional if business allows partial patch).
- CategoryDto / CategoryCreateDto / CategoryUpdateDto
- SupplierDto / SupplierCreateDto / SupplierUpdateDto

Automapper Profiles or manual LINQ projections. Avoid exposing navigation graphs on write endpoints (prevent over-posting). For PATCH, use `JsonPatchDocument<...>` or accept partial update DTO and apply manually.

### 4.4 Validation Rules (Initial Pass)
- Names (ProductName, CategoryName, SupplierName): required, length 1..100.
- ContactName: optional (if schema remains nullable) length <= 100.
- Phone: required, length <= 30, simple regex `^[- +()0-9]{7,30}$` (light validation).
- Price: required >= 0 (if business requires) else optional nullable decimal.
- Stock: required >= 0 integer.

### 4.5 Error Handling
Implement global exception middleware producing RFC 7807 ProblemDetails responses:
- 400 Validation errors ? automatic ProblemDetails.
- 404 Not Found ? produce ProblemDetails with type `https://httpstatuses.com/404`.
- 409 Concurrency conflict (RowVersion) ? type `https://httpstatuses.com/409`.
- 500 Unhandled exceptions ? generic message, correlation id.

### 4.6 Logging
Use Serilog with enrichment (CorrelationId, RequestId). Log at Information for successful requests, Warning for client errors, Error for server faults.

### 4.7 Query Enhancements
Add optional query parameters to Products list:
- filter: nameContains, categoryId, supplierId, minPrice, maxPrice
- sort: field (name|price|stock) + direction (asc|desc)
- pagination: pageNumber (>=1), pageSize (1..100)
Return pagination metadata via custom headers: X-Pagination-TotalCount, X-Pagination-PageSize, etc.

## 5. Migrations & Seeding
Steps:
1. Add initial migration capturing current schema.
2. Add data annotations / fluent config for nullability alignment.
3. Add seed data inside `OnModelCreating` or a `DbInitializer` invoked at startup (idempotent).
4. On application startup, apply migrations automatically in Development / optionally Production with guard.

## 6. Testing Strategy
### 6.1 Integration Tests
- Use a dedicated `CustomWebApplicationFactory` that swaps SQLite with `UseInMemoryDatabase` or `UseSqlite("Filename=:memory:")` keeping same schema.
- Centralize seeding (avoid duplicating nested object graphs).
- Tests cover: CRUD success, validation failures (400), not found (404), cascade deletes (deleting category removes products), concurrency conflicts (409 once RowVersion added).

### 6.2 Unit Tests
- Validate `InventoryDbContext` model configuration (index presence, required fields) using `IMutableModel` metadata.
- Services (if introduced) with mocked DbContext (prefer lightweight in-memory provider or repository abstractions).

### 6.3 Test Tooling
- Add coverage reporting (coverlet) integrated into pipeline.

## 7. Non-Functional Requirements
- Performance: Avoid N+1 queries (use projection or explicit Include). Add `AsNoTracking` for read-only queries.
- Security: Later add Authentication (JWT) + Authorization policies per resource.
- Resilience: Graceful handling of DB unavailability (retry policy could be considered if moved to a server-based DB engine).

## 8. Future Enhancements (Backlog Extract)
- Introduce service layer or CQRS (MediatR) as complexity grows.
- Add OpenAPI client generation (NSwag) for frontend.
- Add caching for reference data (categories, suppliers) (e.g., IMemoryCache) if read patterns justify.
- Add rate limiting middleware.
- Switch to PostgreSQL or SQL Server for production-like features (row version concurrency, transactional semantics) if needed.

## 9. Implementation Order Recommendation
1. Stabilize current schema & nullability (entities + migrations).
2. Introduce DTOs + AutoMapper + Product CRUD.
3. Add global error handling + validation responses.
4. Add Category & Supplier CRUD.
5. Add product_list view endpoint.
6. Add pagination/filter/sort to Product list.
7. Introduce RowVersion concurrency.
8. Expand tests for all endpoints + concurrency scenarios.
9. Logging & structured diagnostics (Serilog) + correlation.
10. Add pipeline (GitHub Actions) with build/test/coverage.

## 10. Coding Conventions (Supplement)
- Follow repository Copilot instructions (camelCase locals/methods, PascalCase types, 2-space indentation, XML docs for public methods, prefix private fields with `_`).
- Use async calls end-to-end (Controller ? EF) with cancellation tokens where applicable.

## 11. Risks & Mitigations
| Risk | Impact | Mitigation |
|------|--------|-----------|
| Over-posting risk on CRUD | Data integrity | Use DTOs with whitelisting & model binding restrictions |
| Concurrency conflicts later | Data loss | Add RowVersion early & test 409 flow |
| Divergence between SQL script & migrations | Deployment failures | Move to migrations as single source & deprecate raw script |
| N+1 query performance issues | Latency | Prefer projection + Include only when needed |
| Integration test data drift | Flaky tests | Central seeding helper reused by tests |

## 12. Open Questions
- Are negative stock values ever valid (backorders)? Currently assumed no.
- Should Price be required at creation? Currently leaning yes.
- Phone normalization rules? Saved as-is; future enhancement: canonical formatting.

## 13. Out of Scope (Current Phase)
- Authentication/Authorization
- Caching layer
- Advanced search (full-text)
- Internationalization / localization

---
This specification will evolve; update sections in tandem with implementation to maintain accuracy.
