# LowKey ('lk') Programming Language

**LowKey ('lk')** is an experimental, clean-syntax programming language built with C as its compilation target. It bridges readable, human-friendly control flows with low-level performance and memory efficiency.

---

## ✨ Features
- **Clean & Bespoke Syntax**: Ditch traditional curly braces for explicit, readable block structures ('if ... do... else ... end').
-  **C Transpilation**: Compiles down to optimized C code for maximum portability and native execution speed.
-  **Type Safety & Scoping**: Powered by a robust static 'SymbolTable' to handle variable scoping, mutation logic, and type integrity.
-  **Minimalist & Lightweight**: Designed from scratch with zero unnecessary bloat.

---

## 🛠️ Syntax Overview

Here is a quick look at how 'lk' handles control flow and mutable variables:

```lk
// Variable declaration and manipulation
int x = 10;
x += 5;

// Conditional logic
if (x > 10) do
  print(x);
else
  print(0);
end

// Loop structure
int count = 0;

while (count < 5) do
  count += 1;
end
```

---

## 🚀 Getting Started

**Prerequisites**
- *GCC / Clang* (or any standard C compiler to compile the generated C code).
- *Current implementation runtime* (temporarily powered by .NET while bootstrapping).

## Building & Running

1. **Clone the repository:**
   ```bash
   git clone https://github.com/c-pers/LowKey-Programming-Language.git
   cd LowKey-Programming-Language
   ```
2. **Build the transpiler:**
   ```bash
   dotnet build
   ```