# Workspace Guidance

This workspace contains three Codex skills:

- `architecture-dot-net-win-client`
- `design-dot-net-win-client`
- `dev-dot-net`

When a task matches one of these areas, use the relevant skill and follow its `SKILL.md` guidance.

## Skill Priority

1. Use `architecture-dot-net-win-client` for solution structure, project boundaries, contracts, tests, and system design.
2. Use `design-dot-net-win-client` for WPF or WinUI UI work, XAML, styling, layout, and window chrome.
3. Use `dev-dot-net` for .NET setup, project files, troubleshooting, validation, and safe file-editing practices.

If a task spans multiple areas, apply all relevant skills in that order.

## Code Quality

Write code that is:

- readable
- maintainable
- structured
- testable

Prefer:

- small, focused units of code
- clear names
- explicit control flow
- separation of concerns
- minimal, intentional abstractions
- configuration over hard-coded values
- sparse, focused comments in the code, only where necessary because of the complexity

Avoid:

- duplicated logic
- unnecessary complexity
- hidden side effects
- large monolithic classes or methods
- UI logic mixed with business logic

## Implementation Rules

- Keep changes consistent with existing repo conventions unless a skill says otherwise.
- Prefer the smallest coherent change that solves the task.
- Add or update tests when behavior changes.
- Preserve ASCII-clean source files unless the repository already uses non-ASCII content.
- Use safe edit workflows for source files.

## Language
Use the English language to create files or modify their content.

## Documentation
Use concise short summaries, when you document something.

## Plan
When updating a plan, do not delete completed tasks, but mark them as fullfilled.
Update the next todo points.
Offer a section for open points / issues, that can be clarified some time later during the development.

