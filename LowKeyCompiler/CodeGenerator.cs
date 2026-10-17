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

        code += NestedCodeGenerator(nodes);

        code += "}";

        return code;
    }

    private string NestedCodeGenerator(List<ASTNode> nodesToCheck)
    {
        string localCode = "";

        foreach (ASTNode node in nodesToCheck)
        {
            if (node is VariableDeclarationNode varDecl)
            {
                string varType = varDecl.DataType == "string" ? "char*" : varDecl.DataType;

                localCode += $"    {varType} {varDecl.VariableName}{(varDecl.InitialValue != null ?
                        $" = {(varType == "char*" ? $"\"{varDecl.InitialValue}\"" : varDecl.InitialValue)};\n" :
                        $";\n")}";
            } else if (node is ConditionalNode condNode)
            {
                localCode += "    if ";
                localCode += ExpressionAnalyzer(condNode);
                localCode += " do\n";

                localCode += NestedCodeGenerator(condNode.ifBranch);
            } else
            {
                Console.WriteLine("Not handled yet.");
            }
        }

        return localCode;
    }

    private string ExpressionAnalyzer(ConditionalNode condNode)
    {
        string localCode = "";

        if (condNode.condition.Left is VariableExpressionNode varExpNode)
        {
            localCode += $"({varExpNode.varName} ";
        }

        localCode += $"{condNode.condition.Operator} ";

        if (condNode.condition.Right is LiteralExpressionNode litExpNode)
        {
            localCode += $"{litExpNode.text})";
        }

        return localCode;
    }
}