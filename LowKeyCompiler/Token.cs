// Everything read by the Lexer returns a Token, composed of its type and a string value
// A list of tokens will be passed to the parser

public enum TokenType
{
    Keyword,
    Identifier,

    String,
    Number,

    Operator,

    RoundBracket,
    Semicolon,

    EOF,
}

public record Token(TokenType type, string value);