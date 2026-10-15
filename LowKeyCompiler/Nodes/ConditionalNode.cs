using System;

public class ConditionalNode : StatementNode
{
    public ExpressionNode condition { get; set; }
    public List<ASTNode> ifBranch { get; set; }
    public List<ASTNode> elseBranch { get; set; }
}