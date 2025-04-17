# Automated SQL Script Analyzer (VSIX Extension for MSSQL)

> A Visual Studio 2017 extension that parses, analyzes, and optimizes large T‑SQL scripts using sliding‑window parsing and OpenAI integration.

---

## 🖥️ Overview

The **Automated SQL Script Analyzer** is a VSIX extension for Microsoft SQL Server that helps you:

- Detect syntax, logic, performance and security issues in long SQL scripts  
- Highlight offending lines and jump straight to them  
- Apply AI‑powered quick‑fixes and optimizations  
- (WIP) Connect to a live database to validate schema, statistics and context  

Built on .NET Framework 4.6+, this extension embeds directly in Visual Studio 2017 and leverages OpenAI to deliver contextual insights and code rewrites.

---

## 📸 screenshots
![image](https://github.com/user-attachments/assets/497b7e8f-c3e0-426d-be88-bcc8ff684ebd)
![image](https://github.com/user-attachments/assets/1c7101fa-577c-4204-af4e-ed4ea08208de)
![image](https://github.com/user-attachments/assets/28f85361-5928-40b3-8b5e-e76aad1d2326)
![image](https://github.com/user-attachments/assets/8131d888-ff3d-4273-99d7-d4d472f117b6)





---

## 🔍 Features

- **AI‑Driven Analysis**  
  Uses OpenAI to understand query intent, detect anti‑patterns, and offer contextual suggestions.  
- **Logical & Syntactical Error Identification**  
  Catch both code‑level syntax mistakes (e.g. missing `SET`) and higher‑level logic flaws (e.g. unreliable `@@ROWCOUNT` usage).  
- **Security Issue Detection**  
  Flag potential SQL injection risks, improper permission checks, and other security anti‑patterns.  
- **Index & Performance Recommendations**  
  Suggest new indexes, warn about missing filters/joins, and highlight expensive scans.  
- **Error‑Line Highlighter**  
  Click any finding card to jump your editor cursor to the exact offending line.  
- **Script Optimization Using AI**  
  “Fix & Optimize” refactors selected snippets, rewriting queries for clarity, safety, and performance.  
- **DB‑Connected Verification (WIP)**  
  Optional live‑database connection for schema and statistics validation, adding an extra layer of context to your analysis.

---

## 🚀 Getting Started

### Prerequisites

- **Visual Studio 2017**  
- **.NET Framework 4.6 or higher**  
- **OpenAI API Key** 

### Installation

1. **Clone the repository**  
   ```bash
   git clone https://github.com/ADHIL007/AutomatedSQLScriptAnalyzer.git
