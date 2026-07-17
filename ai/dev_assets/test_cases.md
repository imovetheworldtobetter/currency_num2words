# **Deutsch – Testfälle semantische Tabellen**

## **DE_0_only**
| Input | expected Output |
|-------|---------|
| 0 | null Dollar |

## **DE_no_comma**
| Input | expected Output |
|-------|---------|
| 33 | dreiunddreißig Dollar |
| 700 | siebenhundert Dollar |
| 1 | ein Dollar |

## **DE_with_comma**
| Input | expected Output |
|-------|---------|
| 14,05 | vierzehn Dollar und fünf Cent |
| 0,50 | null Dollar und fünfzig Cent |
| 33,00 | dreiunddreißig Dollar und null Cent |

## **normalize_leading_zeros_integer**
| Input | Normalisiert | expected Output |
|-------|--------------|--------|
| 0 000,89 | 0,89 | null Dollar und neunundachtzig Cent |
| 0 000,09 | 0,09 | null Dollar und neun Cent |
| 000 567,80 | 567,80 | fünfhundertsiebenundsechzig Dollar und achtzig Cent |

## **normalize_decimal (Zero-Padding bei Kommazahlen)**

| Input | Normalisiert | expected Output |
|-------|--------------|--------|
| 25,1 | 25,10 | fünfundzwanzig Dollar und zehn Cent |
| 0,5 | 0,50 | null Dollar und fünfzig Cent |
| 7, | 7,00 | sieben Dollar und null Cent |

## **normalize_leading_zeros_integer_and_normalize_decimal**
| Input | Normalisiert | expected Output |
|-------|--------------|--------|
| 0000,5 | 0,50 | null Dollar und fünfzig Cent |

## **DE_null_plural_currency**
| Input | expected Output |
|-------|---------|
| 2 | zwei Dollar |
| 2,99 | zwei Dollar und neunundneunzig Cent |
| 100 | einhundert Dollar |

## **DE_capitalization**
| Input | expected Output |
|-------|---------|
| 33 | dreiunddreißig Dollar |
| 14,05 | vierzehn Dollar und fünf Cent |
| 700 | siebenhundert Dollar |

## **DE_concat**
| Input | expected Output |
|-------|---------|
| 33 | dreiunddreißig Dollar |
| 57 | siebenundfünfzig Dollar |
| 999 | neunhundertneunundneunzig Dollar |

## **thousands_space**
| Input | expected Output |
|-------|---------|
| 33 700 | dreiunddreißigtausend siebenhundert Dollar |
| 1 000 | eintausend Dollar |
| 12 345 | zwölftausend dreihundertfünfundvierzig Dollar |



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
| 0 | null | null Dollar | null Dollar und null Cent |
| 1 | ein | ein Dollar | ein Dollar und ein Cent |
| 2 | zwei | zwei Dollar | zwei Dollar und zwei Cent |
| 7 | sieben | sieben Dollar | sieben Dollar und sieben Cent |
| 13 | dreizehn | dreizehn Dollar | dreizehn Dollar und dreizehn Cent |
| 19 | neunzehn | neunzehn Dollar | neunzehn Dollar und neunzehn Cent |

---

## **lookup_tens (20–90)**

| Zahl | Expected Output Zahlenwort | Expected Output Zahlenwort und Währung | Expected Output Vollständiger Geldbetrag in Worten|
|------|--------|--------|--------|
| 20 | zwanzig | zwanzig Dollar | zwanzig Dollar und zwanzig Cent |
| 40 | vierzig | vierzig Dollar | vierzig Dollar und vierzig Cent |
| 90 | neunzig | neunzig Dollar | neunzig Dollar und neunzig Cent |

---

## **lookup_magnitudes**

### **100 – hundert**

| Zahl | Expected Output Zahlenwort | Expected Output Zahlenwort und Währung | Expected Output Vollständiger Geldbetrag in Worten|
|------|--------|--------|--------|
| 100 | einhundert | einhundert Dollar | einhundert Dollar und null Cent |
| 157 | einhundertsiebenundfünfzig | einhundertsiebenundfünfzig Dollar | einhundertsiebenundfünfzig Dollar und null Cent |
| 999 | neunhundertneunundneunzig | neunhundertneunundneunzig Dollar | neunhundertneunundneunzig Dollar und null Cent |

### **1000 – tausend**

| Zahl | Expected Output Zahlenwort | Expected Output Zahlenwort und Währung | Expected Output Vollständiger Geldbetrag in Worten|
|------|--------|--------|--------|
| 1 000 | eintausend | eintausend Dollar | eintausend Dollar und null Cent |
| 1 750 | eintausendsiebenhundertfünfzig | eintausendsiebenhundertfünfzig Dollar | eintausendsiebenhundertfünfzig Dollar und null Cent |
| 9 999 | neuntausendneunhundertneunundneunzig | neuntausendneunhundertneunundneunzig Dollar | neuntausendneunhundertneunundneunzig Dollar und null Cent |

### **1 000 000 – million**

| Zahl | Expected Output Zahlenwort | Expected Output Zahlenwort und Währung | Expected Output Vollständiger Geldbetrag in Worten|
|------|--------|--------|--------|
| 1 000 000 | eine Million | eine Million Dollar | eine Million Dollar und null Cent |
| 1 234 567 | eine Million zweihundertvierunddreißigtausend fünfhundertsiebenundsechzig | eine Million zweihundertvierunddreißigtausend fünfhundertsiebenundsechzig Dollar | eine Million zweihundertvierunddreißigtausend fünfhundertsiebenundsechzig Dollar und null Cent |
| 9 999 999 | neun Millionen neunhundertneunundneunzigtausend neunhundertneunundneunzig | neun Millionen neunhundertneunundneunzigtausend neunhundertneunundneunzig Dollar | neun Millionen neunhundertneunundneunzigtausend neunhundertneunundneunzig Dollar und null Cent |
| 99 999 999 | neunundneunzig Millionen neunhundertneunundneunzigtausend neunhundertneunundneunzig | neunundneunzig Millionen neunhundertneunundneunzigtausend neunhundertneunundneunzig Dollar | neunundneunzig Millionen neunhundertneunundneunzigtausend neunhundertneunundneunzig Dollar und null Cent |

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
