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
- Use `switch` expressions instead of `switch` statements when possible.
