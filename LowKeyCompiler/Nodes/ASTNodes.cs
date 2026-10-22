// All the Abstract Syntax Tree Nodes are declared here

public abstract class ASTNode
{
    
}

public class StatementNode : ASTNode
{
    
}

public class ExpressionNode : ASTNode
{
    
}

public class WhileStatementNode : ASTNode
{
    public ExpressionNode Condition { get; set; }
    public List<ASTNode> Body { get; set; } = new List<ASTNode>();
}

// Nodes inheriting StatementNode

public class VariableDeclarationNode : StatementNode
{
    public string VariableName { get; set; }
    public string DataType {get; set; }
    public ExpressionNode InitialValue { get; set; }
}

public class VariableAssignmentNode : StatementNode
{
    public string VariableName { get; set; }
    public string Operator { get; set; }
    public ExpressionNode ValueExpression { get; set; }
}

public class ConditionalNode : StatementNode
{
    public ExpressionNode Condition { get; set; }
    public List<ASTNode> IfBranch { get; set; } = new List<ASTNode>();
    public List<ASTNode> ElseBranch { get; set; } = new List<ASTNode>();
}

public class PrintStatementNode : StatementNode
{
    public ExpressionNode Expression { get; set; }
}

// Nodes inheriting ExpressionNode

public class LiteralExpressionNode : ExpressionNode
{
    public string Value { get; set; }
    public string Type { get; set; }
}

public class VariableExpressionNode : ExpressionNode
{
    public string VariableName { get; set; }
}

public class BinaryExpressionNode : ExpressionNode
{
    public ExpressionNode Left { get; set; }
    public string Operator { get; set; }
    public ExpressionNode Right { get; set; }
}

public class GroupingExpressionNode : ExpressionNode
{
    public ExpressionNode Expression { get; set; }
}