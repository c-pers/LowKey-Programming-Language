using System;

public class VariableDeclarationNode : StatementNode
{
    public string VariableName { get; set; }
    public string DataType { get; set; }
    public string InitialValue { get; set; }
}