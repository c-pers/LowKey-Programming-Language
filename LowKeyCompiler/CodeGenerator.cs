using System.Xml;

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
            code += GenerateNode(node);
        }

        code += "}";

        return code;
    }

    private string GenerateNode(ASTNode node)
    {
        string code = "";

        if (node is VariableDeclarationNode varDeclNode)
        {
            string varType = varDeclNode.DataType == "string" ? "char*" : varDeclNode.DataType;

            code += $"    {varType} {varDeclNode.VariableName}";

            if (varDeclNode.InitialValue != null)
            {
                code += " = ";
                code += GenerateExpression(varDeclNode.InitialValue);
            }

            code += ";\n";
        } else if (node is VariableAssignmentNode varAssNode)
        {
            code += $"    {varAssNode.VariableName} {varAssNode.Operator} ";
            
            if (varAssNode.ValueExpression is ExpressionNode exprNode)
            {
                code += GenerateExpression(exprNode);
            }

            code += ";\n";
        } else if (node is ConditionalNode condNode)
        {
            code += "    if ";

            if (condNode.Condition is ExpressionNode exprNode)
            {
                code += GenerateExpression(exprNode);
            }

            code += "\n    {\n";

            foreach (ASTNode ifNode in condNode.IfBranch)
            {
                code += $"    {GenerateNode(ifNode)}";
            }

            if (condNode.ElseBranch != null && condNode.ElseBranch.Count > 0)
            {
                code += "    } else\n    {\n";

                foreach (ASTNode elseNode in condNode.ElseBranch)
                {
                    code += $"    {GenerateNode(elseNode)}";
                }
            }

            code += "    }\n";
        } else if (node is PrintStatementNode printNode)
        {
            code += $"    printf({printNode.text});\n";
        }

        return code;
    }

    private string GenerateExpression(ExpressionNode node)
    {
        if (node is LiteralExpressionNode literal)
        {
            return literal.Type == "string" ? $"\"{literal.Value}\"" : literal.Value;
        }

        if (node is VariableExpressionNode variable)
        {
            return variable.VariableName;
        }

        if (node is BinaryExpressionNode binary)
        {
            string leftCode = GenerateExpression(binary.Left);
            string rightCode = GenerateExpression(binary.Right);

            return $"{leftCode} {binary.Operator} {rightCode}";
        }

        if (node is GroupingExpressionNode grouping)
        {
            return $"({GenerateExpression(grouping.Expression)})";
        }

        return "";
    }
}