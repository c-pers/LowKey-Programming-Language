public class CodeGenerator
{
    private readonly List<ASTNode> Nodes;

    public CodeGenerator(List<ASTNode> Nodes)
    {
        this.Nodes = Nodes;
    }

    public string Generate()
    {
        string code = "#include <stdio.h>\n\n" +
                      "int Main()\n" +
                      "{\n";

        foreach (ASTNode node in Nodes)
        {
            if (node is VariableDeclarationNode varDeclNode)
            {
                string varType = varDeclNode.DataType == "string" ? "char*" : varDeclNode.DataType;

                code += $"    {varType} ";
                code += $"{varDeclNode.VariableName} ";
                code += $"= ";
                code += $"{(varDeclNode.InitialValue != null ? 
                    $"{(varType == "char*" ? $"\"{varDeclNode.InitialValue}\"" : $"{varDeclNode.InitialValue}")};\n" : 
                    $";\n")}";
            } else if (node is VariableAssignmentNode varAssNode)
            {
                code += $"    {varAssNode.VariableName} ";
                code += $"{varAssNode.Operator} ";
                code += $"{varAssNode.newValue};\n";
            }
        }

        code += "}";

        return code;
    }
}