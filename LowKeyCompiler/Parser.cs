using System;

public class Parser
{
    // Initialized a read only list of tokens (got from the lexer)
    // and token index
    private readonly List<Token> tokens;

    private int _tokenIndex = 0;

    // Default constructor
    public Parser(List<Token> tokens)
    {
        this.tokens = tokens;
    }

    // Gets the current token from the list and returns it
    private Token PeekToken()
    {
        if (_tokenIndex >= tokens.Count)
        {
            return new Token(TokenType.EOF, "");
        }

        return tokens[_tokenIndex];
    }

    // Consumes the current token and increments the _tokenIndex variable
    private Token ConsumeToken()
    {
        Token currentToken = PeekToken();

        _tokenIndex++;

        return currentToken;
    }

    // Method to verify the given token type is the expected type
    private bool Expect(TokenType type)
    {
        Token currentToken = PeekToken();

        if (currentToken.Type == type)
        {
            ConsumeToken();

            return true;
        }

        Console.WriteLine($"Compiler Error: Expected {type}, got {currentToken.Type}.");

        return false;
    }

    // Main method that runs the parsing of the tokens
    public List<ASTNode> ParseProgram()
    {
        List<ASTNode> nodes = new List<ASTNode>();

        while (PeekToken().Type != TokenType.EOF)
        {
            nodes.Add(ParseStatement());
        }

        return nodes;
    }

    private StatementNode ParseStatement()
    {
        // Peeks the current token
        Token currentToken = PeekToken();

        if (currentToken.Type == TokenType.Keyword)
        {
            // Checks the value and calls the corresponding method
            switch (currentToken.Value)
            {
                case "int" or "string":
                    return ParseVariableDeclaration();
                case "if" or "while":
                    

                    break;
            }
        }

        ConsumeToken();

        return null;
    }

    // Variables parser
    private VariableDeclarationNode ParseVariableDeclaration()
    {
        VariableDeclarationNode varDeclNode = new VariableDeclarationNode();

        varDeclNode.DataType = PeekToken().Value;

        // Consumes the token
        ConsumeToken();

        Token tokenName = PeekToken();
        // Verifies the current token
        if (Expect(TokenType.Identifier))
        {
            varDeclNode.VariableName = tokenName.Value;

            Token opToken = PeekToken();

            // Verifies the current token, an operator or a semicolon is expected
            if (opToken.Type == TokenType.Operator)
            {
                if (opToken.Value == ";")
                {
                    ConsumeToken();
                } else if (opToken.Value == "=")
                {
                    ConsumeToken();

                    Token tokenValue = PeekToken();
                    if (Expect(TokenType.Number))
                    {
                        varDeclNode.InitialValue = tokenValue.Value;

                        if (PeekToken().Value == ";")
                        {
                            ConsumeToken();
                        } else
                        {
                            Console.WriteLine($"Expected ';'. got {PeekToken().Value}.");
                        }
                    }
                }
            } else
            {
                Console.WriteLine($"Expected operator, got {opToken.Type}.");
            }
        }

        return varDeclNode;
    }
}