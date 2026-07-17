# Requirements

## GUI

1. The UI language must be switchable by the user between US-English (US-EN) and German (DE).
2. The default UI language must be US-English (US-EN).
3. The language switch must be displayed as small but clearly visible clickable text in the top-right area of the GUI.
4. The active language must be highlighted while both `DE` and `US-EN` remain visible.
5. The number input field must be horizontally centered and vertically positioned around the golden-ratio area of the window.
6. The input field must be wide enough to hold at least 50 characters. Its height should follow the configured font size.
7. The currently selected currency symbol must be displayed next to the input field.
8. US-EN sets currency to USD and displays `$` on the left side of the input field.
9. German (DE) also sets currency to USD and displays `$` on the left side of the input field.
10. The client must explicitly send both `language` and `currency` to the server.
11. The currency must be set explicitly in client code based on the selected UI language, even though both initial languages currently use USD, so that a later GUI control can change currency independently.
12. The client must send the HTTP header `X-myCurrencyMagic-Client` with conversion requests.
13. The input field must accept only digits `0-9` and comma `,`.
14. Spaces entered by the user must be ignored.
15. The input value must match this basic pattern: 1 to 9 digits, optionally followed by a comma and 1 to 2 digits.
16. The maximum allowed value is `999 999 999,99`.
17. The minimum allowed value is `0`.
18. Only one comma is allowed.
19. If a comma is present, it must have a digit before and after it.
20. Valid examples include `0,3`, `12,00`, and `000,5`.
21. Invalid examples include `,3`, `3,`, `,`, and `,,`.
22. The displayed value must automatically use spaces as thousands separators.
23. Thousands separators must be inserted after each group of three digits before the comma.
24. Example: input `67890` must be displayed as `67 890`.
25. Example: input `2546,36` must be displayed as `2 546,36`.
26. If a comma directly follows three digits, no space must be inserted; `123,45` remains `123,45`.
27. Pasted values such as `1 234,56` must be cleaned automatically by removing spaces and then reformatting the value.
28. Invalid paste operations such as `12,,34`, `abc`, or `12.34` must be rejected.
29. When thousands separators are inserted automatically, the cursor should continue logically so the user can keep typing without jumps.
30. When deleting characters, automatic removal of a thousands separator should not move the cursor to an unexpected position.
31. When inserting a comma, the cursor should remain directly after the comma so the user can enter decimal digits.
32. The input field must allow at least one digit.
33. A single comma is not valid.
34. The input field must show the light-gray visual placeholder format `123 456 789,12`.
35. If the user enters invalid characters, a small accented help text must appear below the input field explaining the allowed characters.
36. If the user enters an invalid format, a small accented help text must appear below the input field explaining the allowed format.
37. A conversion button must be shown below the input field and the validation message area.
38. The conversion button must be left-aligned with the input field.
39. The conversion button text must be `Convert` for US-English and `Umwandeln` for German.
40. If the input characters and format are valid, the conversion button must be enabled.
41. If the input characters or format are invalid, the conversion button must be disabled.
42. A disabled conversion button must be displayed with slight transparency.
43. The conversion button state must follow the current input state.
44. The user must be able to trigger the server-side conversion through the enabled conversion button.
45. If the conversion logic returns a result, the result must be displayed in an output field.
46. If the server or conversion logic is not reachable, a small but clear message must appear to the right of the conversion button.
47. The output field must be below the conversion button.
48. The output field must be left-aligned with the input field.
49. The output field must have the same width as the input field.
50. The output field height must be large enough to display the long English result `nine hundred ninety-nine million nine hundred ninety-nine thousand nine hundred ninety-nine dollars and ninety-nine cents` with a small amount of extra vertical padding.
51. The output field may wrap the number words at spaces so the text fits inside the field.
52. The output field must remain empty until a result is available.
53. When the user changes the UI language, the current input value must be re-sent to the server if it is present and valid so that the output is refreshed in the newly selected language.
54. When the user changes the UI language and the current input is missing or invalid, the output field must be cleared.
55. When the user changes the UI language, the characters in the input field must remain unchanged.
56. A short introductory text must be shown above the input field in a slightly larger font size.
57. The introductory text must explain that the application converts a typed number into a US dollar amount in words and that the result appears in the output field after pressing the conversion button.
58. The introductory text must be German when the UI language is DE.
59. The introductory text must be English when the UI language is US-EN.
60. The user must be able to select and copy the output text through the context menu or `Ctrl+C`.
61. The output field must be read-only.
62. The user must be able to select and copy the input text through the context menu or `Ctrl+C`.
63. The input field must have a small neutral-colored title similar to `Input currency amount as number`.
64. The input field title must be German when the UI language is DE and English when the UI language is US-EN.
65. The output field must have a small neutral-colored title similar to `Output currency amount in words`.
66. The output field title must be German when the UI language is DE and English when the UI language is US-EN.
