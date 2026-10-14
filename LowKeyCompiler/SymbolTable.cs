using System.Collections.Generic;

class SymbolTable
{
    // Dictionary to store variable names and their types
    private Dictionary<string, string> variables = new Dictionary<string, string>();

    // Method to declare a variable with its type
    public bool Declare(string name, string type)
    {
        // Check if the variable already exists in the symbol table
        if (variables.ContainsKey(name))
        {
            return false;
        }

        // Add the variable to the symbol table
        variables[name] = type;

        return true;
    }

    // Method to check if a variable exists in the symbol table
    public bool Exists(string name)
    {
        return variables.ContainsKey(name);
    }

    // Method to get the type of a variable
    public string GetType(string name)
    {
        if (variables.TryGetValue(name, out string type))
        {
            return type;
        }

        return null;
    }
}