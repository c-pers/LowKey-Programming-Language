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

// Nodes inheriting StatementNode

public class VariableDeclarationNode : StatementNode
{
    public string VariableName { get; set; }
    public string DataType {get; set; }
    public string InitialValue { get; set; }
}

public class VariableAssignmentNode : StatementNode
{
    public string VariableName { get; set; }
    public string Operator { get; set; }
    public string newValue { get; set; }
}

public class ConditionalNode : StatementNode
{
    
}

public class PrintStatementNode : StatementNode
{
    
}

// Nodes inheriting ExpressionNode

public class LiteralExpressionNode : ExpressionNode
{
    
}

public class VariableExpressionNode : ExpressionNode
{
    
}

public class BinaryExpressionNode : ExpressionNode
{
    
}