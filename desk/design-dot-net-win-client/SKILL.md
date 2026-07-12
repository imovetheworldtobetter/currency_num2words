---
name: design-dot-net-win-client
description: Design guidance for Codex when building or reviewing .NET Windows client interfaces in WPF or WinUI. Use when creating XAML layouts, styling buttons or input fields, applying the required purple-blue-white brand palette, implementing responsive grid layouts, or building custom window chrome with a gradient header and title-bar controls.
---

# Design Win Client Dot Net

## Core Direction

Apply a clean Windows client interface with generous whitespace, a white background, precise sans-serif typography, angular controls, and a restrained purple-blue brand system.

Use these rules for WPF or WinUI UI work unless the existing application already defines a conflicting design system.

## Color Tokens

Use this palette consistently:

```text
ThemePurple: #41007F
AccentViolet: #7A4DA0
AccentBlueViolet: #AA1DD5
BackgroundWhite: #FFFFFF
PrimaryTextBlue: #0057B8
SecondaryTextGray: #333333
NeutralWhite: #FFFFFF
NeutralDarkGray: #333333
NeutralLightGray: #F5F5F5
```

Primary usage:

- Use `#FFFFFF` as the main window background.
- Use `#0057B8` for primary text.
- Use `#333333` for secondary text.
- Use `#FFFFFF` as the fill color for buttons and input fields.
- Use `#41007F` for button and input borders.
- Use `#F5F5F5` for subtle grouping surfaces only when whitespace alone is not enough.

## Layout

Use a responsive grid as the base layout:

- Narrow width: compact single-column layout.
- Medium width: reduced spacing and tighter grouping.
- Wide width: two-column desktop layout.

Keep the interface spacious. Prefer clear alignment, predictable grid rows and columns, and direct control grouping over decorative containers.

## Typography

Use precise sans-serif typography. Keep text compact and readable:

- Use clear hierarchy through weight, size, and spacing.
- Avoid decorative fonts.
- Avoid oversized text in dense application surfaces.
- Ensure labels, buttons, and validation messages fit without clipping at narrow widths.

## Controls

Style buttons and input fields with:

- White fill.
- Purple border using `#41007F`.
- Angular corners.
- Clear focus and hover states that stay within the palette.

Avoid overly rounded controls. Use square or minimally rounded corners when framework defaults need softening for usability.

## Header And Window Chrome

Include a top bar that is visually present but not too tall. Use this gradient:

```text
#41007F -> #0057B8 -> #AA1DD5
```

For WPF custom chrome:

- Use a top grid as the gradient header.
- Use `WindowChrome` when replacing the native title bar.
- Mark title-bar buttons with `IsHitTestVisibleInChrome` so they remain clickable.
- Support window dragging through `DragMove` from the header area.

Keep title-bar buttons visually consistent with the rest of the UI and ensure they remain accessible against the gradient.

## Review Checklist

When reviewing or modifying a Windows client UI, flag these issues:

- Missing or inconsistent use of the required color palette.
- Non-white main background without a clear reason.
- Rounded controls that conflict with the angular design direction.
- Buttons or fields without the purple border treatment.
- Layout that does not adapt between narrow, medium, and wide widths.
- Header bar that is too tall or lacks the required purple-blue-violet gradient.
- Custom window chrome where title-bar buttons cannot be clicked.
- Custom window chrome where the header cannot drag the window.
