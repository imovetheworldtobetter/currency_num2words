# **Deutsch – Testfälle semantische Tabellen**

## **DE_0_only**
| Input | expected Output |
|-------|---------|
| 0 | null Euro |

## **DE_no_comma**
| Input | expected Output |
|-------|---------|
| 33 | dreiunddreißig Euro |
| 700 | siebenhundert Euro |
| 1 | ein Euro |

## **DE_with_comma**
| Input | expected Output |
|-------|---------|
| 14,05 | vierzehn Euro und fünf Cent |
| 0,50 | null Euro und fünfzig Cent |
| 33,00 | dreiunddreißig Euro und null Cent |

## **normalize_leading_zeros_integer**
| Input | Normalisiert | expected Output |
|-------|--------------|--------|
| 0 000,89 | 0,89 | null Euro und neunundachtzig Cent |
| 0 000,09 | 0,09 | null Euro und neun Cent |
| 000 567,80 | 567,80 | fünfhundertsiebenundsechzig Euro und achtzig Cent |

## **normalize_decimal (Zero-Padding bei Kommazahlen)**

| Input | Normalisiert | expected Output |
|-------|--------------|--------|
| 25,1 | 25,10 | fünfundzwanzig Euro und zehn Cent |
| 0,5 | 0,50 | null Euro und fünfzig Cent |
| 7, | 7,00 | sieben Euro und null Cent |

## **normalize_leading_zeros_integer_and_normalize_decimal**
| Input | Normalisiert | expected Output |
|-------|--------------|--------|
| 0000,5 | 0,50 | null Euro und fünfzig Cent |

## **DE_singular_currency**
| Input | expected Output |
|-------|---------|
| 2 | zwei Euro |
| 2,99 | zwei Euro und neunundneunzig Cent |
| 100 | einhundert Euro |

## **DE_capitalization**
| Input | expected Output |
|-------|---------|
| 33 | dreiunddreißig Euro |
| 14,05 | vierzehn Euro und fünf Cent |
| 700 | siebenhundert Euro |

## **DE_concat**
| Input | expected Output |
|-------|---------|
| 33 | dreiunddreißig Euro |
| 57 | siebenundfünfzig Euro |
| 999 | neunhundertneunundneunzig Euro |

## **thousands_space**
| Input | expected Output |
|-------|---------|
| 33 700 | dreiunddreißigtausend siebenhundert Euro |
| 1 000 | eintausend Euro |
| 12 345 | zwölftausend dreihundertfünfundvierzig Euro |



---

# **Englisch – Testfälle semantische Tabellen**

## **EN_0_only**
| Input | expected Output |
|-------|---------|
| 0 | zero dollars |
| 0 | zero dollars |
| 0 | zero dollars |

## **EN_no_comma**
| Input | expected Output |
|-------|---------|
| 33 | thirty-three dollars |
| 700 | seven hundred dollars |
| 1 | one dollar |

## **EN_with_comma**
| Input | expected Output |
|-------|---------|
| 14,05 | fourteen dollars and five cents |
| 0,50 | zero dollars and fifty cents |
| 33,00 | thirty-three dollars and zero cents |

## **normalize_leading_zeros_integer**
| Input | Normalized | expected Output |
|-------|--------------|--------|
| 0 000,89 | 0,89 | zero dollars and eighty-nine cents |
| 0 000,09 | 0,09 | zero dollars and nine cents |
| 000 567,80 | 567,80 | five hundred sixty-seven dollars and eighty cents |

## **normalize_decimal (Zero-Padding bei Kommazahlen)**

| Input | Normalized | expected Output |
|-------|------------|--------|
| 25,1 | 25,10 | twenty-five dollars and ten cents |
| 0,5 | 0,50 | zero dollars and fifty cents |
| 7, | 7,00 | seven dollars and zero cents |

## **normalize_leading_zeros_integer_and_normalize_decimal**
| Input | Normalized | expected Output |
|-------|------------|--------|
| 0000,5 | 0,50 | zero dollars and fifty cents |

## **EN_plural_currency**
| Input | expected Output |
|-------|---------|
| 2 | two dollars |
| 2,99 | two dollars and ninety-nine cents |
| 100 | one hundred dollars |

## **EN_lowercase**
| Input | expected Output |
|-------|---------|
| 33 | thirty-three dollars |
| 14,05 | fourteen dollars and five cents |
| 700 | seven hundred dollars |

## **EN_hyphen**
| Input | expected Output |
|-------|---------|
| 33 | thirty-three dollars |
| 57 | fifty-seven dollars |
| 99 | ninety-nine dollars |

## **thousands_space**
| Input | expected Output |
|-------|---------|
| 33 700 | thirty-three thousand seven hundred dollars |
| 1 000 | one thousand dollars |
| 12 345 | twelve thousand three hundred forty-five dollars |





# **Deutsch – Testfälle numerische Tabellen**

## **lookup_small (0–19)**

| Zahl | Expected Output Zahlenwort | Expected Output Zahlenwort und Währung | Expected Output Vollständiger Geldbetrag in Worten|
|------|--------|--------|--------|
| 0 | null | null Euro | null Euro und null Cent |
| 1 | ein | ein Euro | ein Euro und ein Cent |
| 2 | zwei | zwei Euro | zwei Euro und zwei Cent |
| 7 | sieben | sieben Euro | sieben Euro und sieben Cent |
| 13 | dreizehn | dreizehn Euro | dreizehn Euro und dreizehn Cent |
| 19 | neunzehn | neunzehn Euro | neunzehn Euro und neunzehn Cent |

---

## **lookup_tens (20–90)**

| Zahl | Expected Output Zahlenwort | Expected Output Zahlenwort und Währung | Expected Output Vollständiger Geldbetrag in Worten|
|------|--------|--------|--------|
| 20 | zwanzig | zwanzig Euro | zwanzig Euro und zwanzig Cent |
| 40 | vierzig | vierzig Euro | vierzig Euro und vierzig Cent |
| 90 | neunzig | neunzig Euro | neunzig Euro und neunzig Cent |

---

## **lookup_magnitudes**

### **100 – hundert**

| Zahl | Expected Output Zahlenwort | Expected Output Zahlenwort und Währung | Expected Output Vollständiger Geldbetrag in Worten|
|------|--------|--------|--------|
| 100 | einhundert | einhundert Euro | einhundert Euro und null Cent |
| 157 | einhundertsiebenundfünfzig | einhundertsiebenundfünfzig Euro | einhundertsiebenundfünfzig Euro und null Cent |
| 999 | neunhundertneunundneunzig | neunhundertneunundneunzig Euro | neunhundertneunundneunzig Euro und null Cent |

### **1000 – tausend**

| Zahl | Expected Output Zahlenwort | Expected Output Zahlenwort und Währung | Expected Output Vollständiger Geldbetrag in Worten|
|------|--------|--------|--------|
| 1 000 | eintausend | eintausend Euro | eintausend Euro und null Cent |
| 1 750 | eintausendsiebenhundertfünfzig | eintausendsiebenhundertfünfzig Euro | eintausendsiebenhundertfünfzig Euro und null Cent |
| 9 999 | neuntausendneunhundertneunundneunzig | neuntausendneunhundertneunundneunzig Euro | neuntausendneunhundertneunundneunzig Euro und null Cent |

### **1 000 000 – million**

| Zahl | Expected Output Zahlenwort | Expected Output Zahlenwort und Währung | Expected Output Vollständiger Geldbetrag in Worten|
|------|--------|--------|--------|
| 1 000 000 | eine Million | eine Million Euro | eine Million Euro und null Cent |
| 1 234 567 | eine Million zweihundertvierunddreißigtausend fünfhundertsiebenundsechzig | eine Million zweihundertvierunddreißigtausend fünfhundertsiebenundsechzig Euro | eine Million zweihundertvierunddreißigtausend fünfhundertsiebenundsechzig Euro und null Cent |
| 9 999 999 | neun Millionen neunhundertneunundneunzigtausend neunhundertneunundneunzig | neun Millionen neunhundertneunundneunzigtausend neunhundertneunundneunzig Euro | neun Millionen neunhundertneunundneunzigtausend neunhundertneunundneunzig Euro und null Cent |
| 99 999 999 | neunundneunzig Millionen neunhundertneunundneunzigtausend neunhundertneunundneunzig | neunundneunzig Millionen neunhundertneunundneunzigtausend neunhundertneunundneunzig Euro | neunundneunzig Millionen neunhundertneunundneunzigtausend neunhundertneunundneunzig Euro und null Cent |

---

# **Englisch – Testfälle numerische Tabellen**

## **lookup_small (0–19)**

| Number | Expected Output number word | Expected Output number word and currency | Expected Output complete amount of money in words|
|--------|--------|--------|--------|
| 0 | zero | zero dollars | zero dollars and zero cents |
| 1 | one | one dollar | one dollar and one cent |
| 2 | two | two dollars | two dollars and two cents |
| 7 | seven | seven dollars | seven dollars and seven cents |
| 13 | thirteen | thirteen dollars | thirteen dollars and thirteen cents |
| 19 | nineteen | nineteen dollars | nineteen dollars and nineteen cents |

---

## **lookup_tens (20–90)**

| Number | Expected Output number word | Expected Output number word and currency | Expected Output complete amount of money in words|
|--------|--------|--------|--------|
| 20 | twenty | twenty dollars | twenty dollars and twenty cents |
| 40 | forty | forty dollars | forty dollars and forty cents |
| 90 | ninety | ninety dollars | ninety dollars and ninety cents |

---

## **lookup_magnitudes**

### **100 – hundred**

| Number | Expected Output number word | Expected Output number word and currency | Expected Output complete amount of money in words|
|--------|--------|--------|--------|
| 100 | one hundred | one hundred dollars | one hundred dollars and zero cents |
| 157 | one hundred fifty-seven | one hundred fifty-seven dollars | one hundred fifty-seven dollars and zero cents |
| 999 | nine hundred ninety-nine | nine hundred ninety-nine dollars | nine hundred ninety-nine dollars and zero cents |

### **1000 – thousand**

| Number | Expected Output number word | Expected Output number word and currency | Expected Output complete amount of money in words|
|--------|--------|--------|--------|
| 1 000 | one thousand | one thousand dollars | one thousand dollars and zero cents |
| 1 750 | one thousand seven hundred fifty | one thousand seven hundred fifty dollars | one thousand seven hundred fifty dollars and zero cents |
| 9 999 | nine thousand nine hundred ninety-nine | nine thousand nine hundred ninety-nine dollars | nine thousand nine hundred ninety-nine dollars and zero cents |

### **1 000 000 – million**

| Number | Expected Output number word | Expected Output number word and currency | Expected Output complete amount of money in words|
|--------|--------|--------|--------|
| 1 000 000 | one million | one million dollars | one million dollars and zero cents |
| 1 234 567 | one million two hundred thirty-four thousand five hundred sixty-seven | one million two hundred thirty-four thousand five hundred sixty-seven dollars | one million two hundred thirty-four thousand five hundred sixty-seven dollars and zero cents |
| 9 999 999 | nine million nine hundred ninety-nine thousand nine hundred ninety-nine | nine million nine hundred ninety-nine thousand nine hundred ninety-nine dollars | nine million nine hundred ninety-nine thousand nine hundred ninety-nine dollars and zero cents |
| 99 999 999 | ninety-nine million nine hundred ninety-nine thousand nine hundred ninety-nine | ninety-nine million nine hundred ninety-nine thousand nine hundred ninety-nine dollars | ninety-nine million nine hundred ninety-nine thousand nine hundred ninety-nine dollars and zero cents |