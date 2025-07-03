---
applyTo: '**/*.cs'
---
# Code Style Instructions

## Namespaces
- Use file scoped namespaces that match the folder structure.

## Code Style
- Use the target-typed new() when possible.
- Place all local functions after a return statement.
- inner classes or structs should be placed at the top of the file.

## Comments
- Comments should be used to explain the why, not the how. Let the code be self-documenting.

## Conditional Statements
- `if` statements that contain a single statement should not use braces. The single statement should be on the next line and indented.
- For single statement `ifs`, always add a blank line before and after.
- Use `switch` expressions instead of `switch` statements when possible.
- Prefer early returns in methods to reduce nesting.

## Properties
- Use `get; init;` for properties that should be immutable after initialization.
- Use `get; set;` for properties that can be modified after initialization.
- Use `private set;` for properties that should only be set within the class.
- Never define public fields. Always use properties.

## Classes and Structs
- When value semantics are required, use records.

## Naming Conventions
- Use PascalCase for public members, properties, and methods.
- Use camelCase for method parameters and local variables.
- Use _camelCase for private fields.
- Use UPPER_CASE for constants.
- Use meaningful names that clearly describe the purpose of the variable, method, or class.
- Avoid abbreviations unless they are well-known acronyms.
