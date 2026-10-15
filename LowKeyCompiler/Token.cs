using System;

public enum TokenType
{
    Keyword,
    Identifier,
    String,
    Number,
    Operator,
    Symbol,
    Semicolon,
    RoundBracket,
    EOF
}

public record Token(TokenType Type, string Value);