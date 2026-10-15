public enum TokenType
{
    Keyword,
    Identifier,
    Number,
    Operator,
    Symbol,
    EOF
}

public record Token(TokenType Type, string Value);