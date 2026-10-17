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

    private string GetOperators(string givenOperator)
    {
        string opString = givenOperator;

        while (_tokenIndex < tokens.Count && PeekToken().Value == givenOperator) {
            opString += PeekToken().Value;

            ConsumeToken();
        }

        return opString;
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
                case "if":
                    return ParseConditional();
                case "print":
                    break;
            }
        }

        ConsumeToken();

        return null;
    }

    // Conditional parser (if statement)
    private ConditionalNode ParseConditional()
    {
        ConditionalNode condNode = new ConditionalNode();

        //  TODO

        ConsumeToken();

        Token currentToken = ConsumeToken();
        if (currentToken.Value == "(")
        {
            BinaryExpressionNode condition = new BinaryExpressionNode();

            currentToken = PeekToken();

            while (_tokenIndex < tokens.Count && currentToken.Value != ")")
            {
                if (currentToken.Type == TokenType.Identifier)
                {
                    ConsumeToken();

                    VariableExpressionNode left = new VariableExpressionNode();

                    left.varName = currentToken.Value;

                    condition.Left = left;

                    currentToken = PeekToken();

                    if (currentToken.Type == TokenType.Symbol)
                    {
                        ConsumeToken();

                        string op = GetOperators(currentToken.Value);

                        condition.Operator = op;

                        currentToken = PeekToken();

                        Console.WriteLine(currentToken.Type.ToString());

                        if (currentToken.Type == TokenType.Number)
                        {
                            ConsumeToken();

                            LiteralExpressionNode right = new LiteralExpressionNode();

                            right.text = currentToken.Value.ToString();

                            condition.Right = right;
                        }
                    }
                } else
                {
                    ConsumeToken();
                }

                currentToken = PeekToken();
            }

            if (PeekToken().Value == ")")
            {
                ConsumeToken();
            } else
            {
                Console.WriteLine("Compiler Error: Expected ')' after if condition.");

                return null;
            }

            condNode.condition = condition;
        } else
        {
            Console.WriteLine("Compiler Error: Expected '(' after 'if'.");

            return null;
        }

        List<ASTNode> conditionNodes = new List<ASTNode>();

        while (_tokenIndex < tokens.Count && (currentToken.Value != "else" && currentToken.Value != "end"))
        {
            ConsumeToken();

            conditionNodes.Add(ParseStatement());
        }

        condNode.ifBranch = conditionNodes;

        if (currentToken.Value == "else")
        {
            List<ASTNode> elseConditionNodes = new List<ASTNode>();

            while (_tokenIndex < tokens.Count && currentToken.Value != "end")
            {
                ConsumeToken();

                elseConditionNodes.Add(ParseStatement());
            }

            condNode.elseBranch = elseConditionNodes;
        }

        return condNode;
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

            Token nextToken = PeekToken();

            // Verifies the current token, an operator or a semicolon is expected
            if (nextToken.Type == TokenType.Semicolon)
            {
                ConsumeToken();
            } else if (nextToken.Type == TokenType.Symbol)
            {
                if (nextToken.Value == "=")
                {
                    ConsumeToken();

                    Token tokenValue = PeekToken();
                    if (tokenValue.Type == TokenType.Number || tokenValue.Type == TokenType.String)
                    {
                        ConsumeToken();
                        
                        varDeclNode.InitialValue = tokenValue.Value;

                        if (PeekToken().Type == TokenType.Semicolon)
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
                ConsumeToken();

                Console.WriteLine($"Expected operator, got {nextToken.Type}.");
            }
        }

        return varDeclNode;
    }
}