using System;
using System.IO;
using System.Text.RegularExpressions;

class Program
{
    static void Main()
    {
        // Check if the input file exists
        if (!File.Exists("test.lk"))
        {
            Console.WriteLine("Error: file 'test.lk' not found.");
            return;
        }

        SymbolTable SymbolTable = new SymbolTable();

        string[] lines = File.ReadAllLines("test.lk");

        string code = "#include <stdio.h>\n\n" +
                      "int main() {\n";

        bool hasErrors = false;

        // Process each line of the input file
        foreach (string rawLine in lines)
        {
            string line = rawLine.Trim();

            // Skip empty lines
            if (string.IsNullOrEmpty(line)) continue;

            // Match variable declarations and print statements

            var varMatch = Regex.Match(line, @"^\s*([a-zA-Z]+)\s+([a-zA-Z]\w+)\s*=\s*(.+)\s*;\s*$");

            var printMatch = Regex.Match(line, @"print\s*\(\s*(.+)\s*\)\s*;");

            var ifMatch = Regex.Match(line, @"^\s*if\s*\((.+)\)\s*do\s*$");
            var elseMatch = Regex.Match(line, @"^\s*else\s*$");
            var endMatch = Regex.Match(line, @"^\s*end\s*$");

            var whileMatch = Regex.Match(line, @"^\s*while\s*\((.+)\)\s*do\s*$");

            var varSettingMatch = Regex.Match(line, @"^\s*([a-zA-Z]\w+)\s*(.+)\s*(.+)\s*;\s*$");

            // Handle print statements and variable declarations

            // TODO: Handle string "+" and "${variable}"
            if (printMatch.Success)
            {
                string printData = printMatch.Groups[1].Value;

                // Validate print statement
                if (Regex.IsMatch(printData, @"^\d+$"))
                {
                    code += $"    printf(\"%d\\n\", {printData});\n";
                } else if (Regex.IsMatch(printData, @"^""([^""]*)""$"))
                {
                    code += $"    printf(\"%s\\n\", {printData});\n";
                } else if (Regex.IsMatch(printData, @"^[a-zA-Z]\w*$"))
                {
                    string varName = printData;

                    if (SymbolTable.Exists(varName))
                    {
                        string varType = SymbolTable.GetType(varName);

                        if (varType == "int")
                        {
                            code += $"    printf(\"%d\\n\", {varName});\n";
                        } else if (varType == "decimal")
                        {
                            code += $"    printf(\"%lf\\n\", {varName});\n";
                        } else if (varType == "string")
                        {
                            code += $"    printf(\"%s\\n\", {varName});\n";
                        }
                    } else
                    {
                        Console.WriteLine($"Compiler Error: Use of undeclared variable '{varName}' in print statement.");

                        hasErrors = true;
                    }
                } else
                {
                    Console.WriteLine($"Compilation Error: Invalid print statement {line}.");

                    hasErrors = true;
                }
            } else if (varMatch.Success)
            {
                bool isValid = false;

                string type = varMatch.Groups[1].Value;
                string name = varMatch.Groups[2].Value;
                string rawValue = varMatch.Groups[3].Value;

                // Validate the type and value
                if (type == "int")
                {
                    if (Regex.IsMatch(rawValue, @"^\d+$"))
                    {
                        code += $"    int {name} = {varMatch.Groups[3].Value};\n\n";

                        isValid = true;
                    } else
                    {
                        Console.WriteLine($"Compiler Error: Type mismatch. Expected an integer for '{name}', got '{rawValue}'.");

                        hasErrors = true;
                    }
                } else if (type == "decimal")
                {
                    if (Regex.IsMatch(rawValue, @"^\d+(\.\d+)?$"))
                    {
                        code += $"    double {name} = {varMatch.Groups[3].Value};\n";

                        isValid = true;
                    } else
                    {
                        Console.WriteLine($"Compiler Error: Type mismatch. Expected a decimal for '{name}', got '{rawValue}'.");

                        hasErrors = true;
                    }
                } else if (type == "string")
                {
                    if (Regex.IsMatch(rawValue, @"^""([^""]*)""$"))
                    {
                        code += $"    const char* {name} = {varMatch.Groups[3].Value};\n";

                        isValid = true;
                    } else
                    {
                        Console.WriteLine($"Compiler Error: Type mismatch. Expected a string for '{name}', got '{rawValue}'.");

                        hasErrors = true;
                    }
                } else
                {
                    Console.WriteLine($"Compiler Error: Unknown type '{type}'.");

                    hasErrors = true;
                }

                if (isValid)
                {
                    // Check if the variable has already been declared
                    if (!SymbolTable.Declare(name, type))
                    {
                        Console.WriteLine($"Compiler Error: Variable '{name}' is already declared.");

                        hasErrors = true;
                    }
                }
            } else if (varSettingMatch.Success)
            {
                string varName = varSettingMatch.Groups[1].Value;

                if (!SymbolTable.Exists(varName))
                {
                    Console.WriteLine($"Compiler Error: Use of undeclared variable '{varName}'.");

                    hasErrors = true;
                } else
                {
                    string assignment = varSettingMatch.Groups[2].Value;
                    string value = varSettingMatch.Groups[3].Value;

                    if (Regex.IsMatch(assignment, @"^\s*=\s*$"))
                    {
                        code += $"    {varName} = {value};\n";
                    } else if (Regex.IsMatch(assignment, @"^\s*\+=\s*$"))
                    {
                        code += $"    {varName} += {value};\n";
                    } else if (Regex.IsMatch(assignment, @"^\s*-=\s*$"))
                    {
                        code += $"    {varName} -= {value};\n";
                    }
                }
            } else if (whileMatch.Success)
            {
                string condition = whileMatch.Groups[1].Value;

                code += $"    while ({condition}) {{\n";
            } else if (ifMatch.Success)
            {
                string condition = ifMatch.Groups[1].Value;

                code += $"    if ({condition}) {{\n";
            } else if (elseMatch.Success)
            {
                code += "    } else {\n";
            } else if (endMatch.Success)
            {
                code += "    }\n";
            } else
            {
                Console.WriteLine($"Syntax error: '{line}'");

                hasErrors = true;
            }
        }

        code += "}\n";

        // Write the generated C code to an output file if there are no errors
        if (!hasErrors)
        {
            File.WriteAllText("output.c", code);
            Console.WriteLine("Compilation successful. Output written to 'output.c'.");
        } else
        {
            Console.WriteLine("[ERROR] Compilation failed due to errors.");
        }
    }
}