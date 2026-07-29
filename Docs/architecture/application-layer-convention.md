# Application Layer Convention

## 1. Architecture Foundation

This project strictly adheres to the following architectural patterns and organizational strategies:

*   **Clean Architecture**
*   **CQRS** (Command Query Responsibility Segregation)
*   **MediatR**
*   **Feature-Based Organization**

These architectural decisions are fixed and form the foundation of the Application layer's structure.

---

## 2. Official Folder Convention

Every standard feature must follow this exact directory layout:

```text
Feature/
│
├── Commands/
├── Queries/
├── Handlers/
├── DTOs/
├── Validators/      (Optional)
└── EventHandlers/   (Optional)
```

---

## 3. Structural Rules

### Commands
*   **Purpose:** Commands contain only command models (the intent to change state).
*   **Location:** Directly inside the `Commands/` folder.
*   **Restriction:** There is no `Models` sub-folder.
*   **Rule:** One command per file.

**Example:**
```text
Commands/
    CreateProductCommand.cs
```

### Queries
*   **Purpose:** Queries contain only query models (the intent to read state).
*   **Location:** Directly inside the `Queries/` folder.
*   **Restriction:** There is no `Models` sub-folder.
*   **Rule:** One query per file.

### Handlers
*   **Purpose:** Handlers contain the execution logic for Commands and Queries.
*   **Location:** All handlers must be located under the `Handlers/` folder.
*   **Restriction:** Never place handlers inside the `Commands/` or `Queries/` folders.
*   **Rule:** One handler per file.

### DTOs
*   **Purpose:** Data Transfer Objects used for request/response payloads.
*   **Location:** DTOs remain strictly inside `Feature/DTOs`.
*   **Restriction:** DTOs should not be shared across unrelated features unless they represent a genuine, documented cross-feature contract.

### Validators
*   **Purpose:** Validation rules for Commands and Queries.
*   **Location:** Validators live in `Feature/Validators`.
*   **Rule:** The `Validators/` folder is optional. Create it only if the feature requires validation.
*   **Rule:** One validator per file.

### Event Handlers
*   **Purpose:** Logic that responds to domain or integration events.
*   **Location:** Event handlers live in `Feature/EventHandlers`.
*   **Rule:** The `EventHandlers/` folder is optional. Create it only when the feature actually contains one or more event handlers. Do not create empty folders.

---

## 4. Multi-Entity Features Exception

The Catalog module is an approved architectural exception because it manages multiple tightly coupled entities (e.g., Product and Category).

Entity-based subfolders are recommended when they improve organization and clarity.

Commands, Queries, and Handlers may exist either directly under their respective folders or inside entity-specific subfolders, depending on which structure provides the clearest organization.

Avoid unnecessary nesting.

No other feature should introduce similar nesting unless there is a documented architectural justification.

---

## 5. Naming Convention

Names must remain consistent across the entire solution.

### Commands

CRUD operations should follow consistent naming.

Examples:

- Create<Entity>Command
- Update<Entity>Command
- Delete<Entity>Command

Domain-specific operations must preserve the ubiquitous language of the business domain.

Examples:

- PayInvoiceCommand
- AdjustStockCommand
- ApprovePurchaseOrderCommand
- ReceivePurchaseOrderCommand
- CancelOrderCommand

Do not recommend renaming domain-specific commands simply to match CRUD prefixes.

**Queries:**
*   `Get<Entity>ByIdQuery`
*   `Get<Entities>Query` (for collections)

---

## 6. Coding Standards

### Namespaces

Namespaces must always mirror the physical folder structure.

Whenever a file or folder is moved, its namespace must be updated accordingly.

The project should never contain namespaces that no longer match the directory layout.

### One Class Per File
Every architectural component must live in its own dedicated file. This applies to:
*   Commands
*   Queries
*   Handlers
*   Validators
*   DTOs

**Prohibited:** Grouped or pluralized files (e.g., `CustomerCommands.cs`, `RoleHandlers.cs`) are not allowed.

### Cache Keys
All cache keys must be centralized.
*   **Location:** `Common/Caching/CacheKeys.cs`
*   **Restriction:** Hardcoded cache key string literals are strictly prohibited inside handlers or queries.

### Folder Creation Rule
Do not create folders containing only one file unless the folder represents a meaningful architectural concept (like `Commands` or `Queries`) or is expected to grow naturally over time. Avoid unnecessary and deep folder nesting.

---

## 7. Guiding Principles

The primary goals of this convention are:
*   **Consistency**
*   **Predictability**
*   **Simplicity**
*   **Low cognitive load**
*   **Low refactoring cost**
*   **High maintainability**

Consistency is always preferred over introducing new structural patterns. Any exception to this convention must be documented with a clear architectural justification.
