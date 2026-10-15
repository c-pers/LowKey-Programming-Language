using System;

public abstract class ExpressionNode : ASTNode
{
    
}

public class BinaryExpressionNode : ExpressionNode
{
    public ExpressionNode Left { get; set; }
    public string Operator { get; set; }
    public ExpressionNode Right { get; set; }
}

public class VariableExpressionNode : ExpressionNode
{
    public string varName { get; set; }
}

public class LiteralExpressionNode : ExpressionNode
{
    public string text { get; set; }
}