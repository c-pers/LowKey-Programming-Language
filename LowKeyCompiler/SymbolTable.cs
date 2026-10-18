public class SymbolTable
{
    public Dictionary<string, string> _variables = new Dictionary<string, string>();

    public bool Declare(string name, string type)
    {
        if (_variables.ContainsKey(name))
        {
            return false;
        }

        _variables[name] = type;

        return true;
    }

    public bool Exists(string name)
    {
        return _variables.ContainsKey(name);
    }

    public string GetType(string name)
    {
        return _variables.ContainsKey(name) ? _variables[name] : null;
    }
}