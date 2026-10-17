using System;

public class ConditionalNode : StatementNode
{
    public BinaryExpressionNode condition { get; set; }
    public List<ASTNode> ifBranch { get; set; }
    public List<ASTNode> elseBranch { get; set; }
}