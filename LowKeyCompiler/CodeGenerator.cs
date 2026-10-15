using System;

class CodeGenerator
{
    private readonly List<ASTNode> nodes;
    public CodeGenerator(List<ASTNode> nodes)
    {
        this.nodes = nodes;
    }

    public string Generate()
    {
        string code = "#include <stdio.h>\n\n" +
                      "int Main()\n" +
                      "{\n";

        foreach (ASTNode node in nodes)
        {
            if (node is VariableDeclarationNode varDecl)
            {
                string varType = varDecl.DataType == "string" ? "char*" : varDecl.DataType;

                code += $"    {varType} {varDecl.VariableName}{(varDecl.InitialValue != null ?
                        $" = {(varType == "char*" ? $"\"{varDecl.InitialValue}\"" : varDecl.InitialValue)};\n" :
                        $";\n")}";
            }
        }

        code += "}";

        return code;
    }
}