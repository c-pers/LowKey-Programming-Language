// The Parser aims to get the list of tokens returned by the lexer and to convert everything to Abstract Syntax Tree nodes

using System.Runtime.InteropServices;

public class Parser
{
    private readonly List<Token> Tokens;
    private int _tokenIndex = 0;

    private SymbolTable symbolTable = new SymbolTable();

    private string[] VariableDeclarationKeywords =
    {
      "int",
      "string",
      "decimal",
    };

    public Parser(List<Token> tokens)
    {
        Tokens = tokens;
    }

    private string ExpressionToString(ExpressionNode node)
    {
        string convertedStr = "";

        if (node is VariableExpressionNode varExprNode)
        {
            convertedStr += $"{varExprNode.VariableName}";
        } else if (node is BinaryExpressionNode binExprNode)
        {
            string left = ExpressionToString(binExprNode.Left);

            string op = $"{binExprNode.Operator}";

            string right = ExpressionToString(binExprNode.Right);

            convertedStr += $"{left} {op} {right}";
        } else if (node is GroupingExpressionNode groupExprNode)
        {
            string groupStr = ExpressionToString(groupExprNode.Expression);

            convertedStr += $"({groupStr})";
        } else if (node is LiteralExpressionNode litExprNode)
        {
            convertedStr += $"{litExprNode.Value}";
        }
        
        return convertedStr;
    }

    // Peeks the current token
    private Token Peek()
    {
        if (_tokenIndex >= Tokens.Count)
        {
            return new Token(TokenType.EOF, "");
        }

        Token currentToken = Tokens[_tokenIndex];

        return currentToken;
    }

    // Returns the current token and changes the pointer to the next one
    // It will always return the peeked token before the iteration
    private Token Consume(int? repeat = 1)
    {
        Token currentToken = Peek();

        for (int i = 0; i < repeat; i++)
        {
            _tokenIndex++;
        }

        return currentToken;
    }

    // Helper method to check if the current token type is the expected one
    private bool Expect(TokenType type)
    {
        if (Peek().type == type)
        {
            return true;
        }

        return false;
    }

    // Main method to parse the entire file
    public List<ASTNode> ParseProgram()
    {
        List<ASTNode> Nodes = new List<ASTNode>();

        // Runs until the current token type isn't the end of the file
        while (Peek().type != TokenType.EOF)
        {
            Nodes.Add(Parse());
        }

        return Nodes;
    }

    // Responsible of checking the current token's type and calling the respective method
    private ASTNode Parse()
    {
        Token currentToken = Peek();

        if (currentToken.type == TokenType.Keyword)
        {
            if (VariableDeclarationKeywords.Contains(currentToken.value))
            {
                return ParseVariableDeclaration();
            } else if (currentToken.value == "if")
            {
                return ParseCondition();
            } else if (currentToken.value == "print")
            {
                return ParsePrint();
            }
        } else if (currentToken.type == TokenType.Identifier)
        {
            return ParseVariableAssignment();
        }

        return null;
    }

    // Expressions

    private ExpressionNode ParseExpression()
    {
        return ParseComparison();
    }

    private ExpressionNode ParseComparison()
    {
        ExpressionNode left = ParseAdditive();

        string[] comparisonOperators = { "==", "<", ">", "<=", ">=" };

        while (comparisonOperators.Contains(Peek().value))
        {
            string op = Consume().value;

            ExpressionNode right = ParseAdditive();

            left = new BinaryExpressionNode { Left = left, Operator = op, Right = right };
        }

        return left;
    }

    private ExpressionNode ParseAdditive()
    {
        ExpressionNode left = ParseMultiplicative();

        while (Peek().value == "+" || Peek().value == "-")
        {
            string op = Consume().value;

            ExpressionNode right = ParseMultiplicative();

            left = new BinaryExpressionNode { Left = left, Operator = op, Right = right };
        }

        return left;
    }

    private ExpressionNode ParseMultiplicative()
    {
        ExpressionNode left = ParsePrimary();

        while (Peek().value == "*" || Peek().value == "/")
        {
            string op = Consume().value;

            ExpressionNode right = ParsePrimary();

            left = new BinaryExpressionNode { Left = left, Operator = op, Right = right };
        }

        return left;
    }

    private ExpressionNode ParsePrimary()
    {
        Token currentToken = Peek();

        if (currentToken.type == TokenType.Number)
        {
            Consume();

            return new LiteralExpressionNode { Value = currentToken.value, Type = "number" };
        }

        if (currentToken.type == TokenType.String)
        {
            Consume();

            return new LiteralExpressionNode { Value = currentToken.value, Type = "string" };
        }

        if (currentToken.type == TokenType.Identifier)
        {
            Consume();

            return new VariableExpressionNode { VariableName = currentToken.value };
        }

        if (currentToken.value == "(")
        {
            Consume();

            ExpressionNode expr = ParseExpression();

            if (Peek().value == ")")
            {
                Consume();
            } else
            {
                Console.WriteLine($"Compiler Error: Expected ')' after expression, got {Peek().value}.");

                return null;
            }

            return new GroupingExpressionNode { Expression = expr };
        }

        Console.WriteLine($"Compiler Error: Unexpected token '{currentToken.value}' in expression.");

        return null;
    }

    // Statements

    // Responsible for variable declarations (e.g. int x = 1)
    private VariableDeclarationNode ParseVariableDeclaration()
    {
        VariableDeclarationNode varDeclNode = new VariableDeclarationNode();

        string varType = Peek().value;

        varDeclNode.DataType = Peek().value;

        Consume();

        Token currentToken = Peek();

        if (Expect(TokenType.Identifier))
        {
            varDeclNode.VariableName = currentToken.value;

            if (!symbolTable.Declare(varDeclNode.VariableName, varType))
            {
                Console.WriteLine($"Compiler Error: A variable named '{varDeclNode.VariableName}' already exists.");

                return null;
            }

            Consume();

            if (Peek().value == "=")
            {
                Consume();

                currentToken = Peek();

                ExpressionNode expr = ParseExpression();

                varDeclNode.InitialValue = expr;

                if (!Expect(TokenType.Semicolon))
                {
                    Console.WriteLine("Compiler Error: Expected ';'.");

                    return null;
                } else
                {
                    Consume();
                }
            } else if (Peek().value == ";")
            {
                Consume();
            }
        } else
        {
            Console.WriteLine($"Compiler Error: Expected an identifier, got '{currentToken.type}'.");

            return null;
        }

        return varDeclNode;
    }

    // Responsible for variable assignment (e.g. x = 5 + 10)
    private VariableAssignmentNode ParseVariableAssignment()
    {
        VariableAssignmentNode varAssNode = new VariableAssignmentNode();

        if (Expect(TokenType.Identifier))
        {
            varAssNode.VariableName = Peek().value;

            Consume();

            string varType;

            if (symbolTable.Exists(varAssNode.VariableName))
            {
                varType = symbolTable.GetType(varAssNode.VariableName);
            } else
            {
                Console.WriteLine($"Compiler Error: No variable named '{varAssNode.VariableName}' found. Have you declared it?");

                return null;
            }

            if (Expect(TokenType.Operator))
            {
                varAssNode.Operator = Peek().value;

                Consume();

                ExpressionNode expr = ParseExpression();

                varAssNode.ValueExpression = expr;

                if (Expect(TokenType.Semicolon))
                {
                    Consume();
                } else
                {
                    Console.WriteLine($"Compiler Error: ';' expected, got '{Peek().value}'.");

                    return null;
                }
            } else
            {
                Console.WriteLine($"Compiler Error: Operator expected, got '{Peek().type}'.");

                return null;
            }
        } else
        {
            Console.WriteLine($"Compiler Error: Variable identifier expected, got '{Peek().type}'.");

            return null;
        }

        return varAssNode;
    }

    private ConditionalNode ParseCondition()
    {
        ConditionalNode condNode = new ConditionalNode();

        if (Peek().value == "if")
        {
            Consume();

            ExpressionNode expr = ParseExpression();

            condNode.Condition = expr;

            if (Peek().value == "do")
            {
                Consume();

                while (Peek().type != TokenType.EOF && Peek().value != "end" && Peek().value != "else")
                {
                    ASTNode stmt = Parse();

                    if (stmt != null)
                    {
                        condNode.IfBranch.Add(stmt);
                    } else
                    {
                        Consume();
                    }
                }
            }
        }

        if (Peek().value == "else")
        {
            Consume();

            if (Peek().value == "if")
            {
                // Nothing for now
            } else
            {
                while (Peek().type != TokenType.EOF && Peek().value != "end")
                {
                    ASTNode stmt = Parse();

                    if (stmt != null)
                    {
                        condNode.ElseBranch.Add(stmt);
                    } else
                    {
                        Consume();
                    }
                }
            }
        }

        if (Peek().value == "end")
        {
            Consume();
        } else
        {
            Console.WriteLine($"Compiler Error: Expected 'end', got '{Peek().value}'.");

            return null;
        }

        return condNode;
    }

    private PrintStatementNode ParsePrint()
    {
        PrintStatementNode printNode = new PrintStatementNode();

        Consume();

        if (Expect(TokenType.RoundBracket))
        {
            Consume();

            if (Peek().type == TokenType.String)
            {
                string str = "";

                while (Peek().type != TokenType.EOF && Peek().type == TokenType.String)
                {
                    str += Peek().value;

                    Consume();
                }

                printNode.text = $"\"{str}\"";

                if (Expect(TokenType.RoundBracket))
                {
                    Consume();
                } else
                {
                    Console.WriteLine($"Compiler Error: Expected ')', got {Peek().value}.");

                    return null;
                }

                if (Expect(TokenType.Semicolon))
                {
                    Consume();
                } else
                {
                    Console.WriteLine($"Compiler Error: Expected ';', got {Peek().value}.");

                    return null;
                }
            } else if (Expect(TokenType.Identifier))
            {
                ExpressionNode printStmt = ParseExpression();

                string str = ExpressionToString(printStmt);

                if (str.Length > 0)
                {
                    printNode.text = str;
                } else
                {
                    return null;
                }

                if (Expect(TokenType.RoundBracket))
                {
                    Consume();
                } else
                {
                    Console.WriteLine($"Compiler Error: Expected ')', got {Peek().value}.");

                    return null;
                }

                if (Expect(TokenType.Semicolon))
                {
                    Consume();
                } else
                {
                    Console.WriteLine($"Compiler Error: Expected ';', got {Peek().value}.");

                    return null;
                }
            } else if (Expect(TokenType.Number))
            {
                ExpressionNode printStmt = ParseExpression();

                string str = ExpressionToString(printStmt);

                if (str.Length > 0)
                {
                    printNode.text = str;
                } else
                {
                    return null;
                }

                if (Expect(TokenType.RoundBracket))
                {
                    Consume();
                } else
                {
                    Console.WriteLine($"Compiler Error: Expected ')', got {Peek().value}.");

                    return null;
                }

                if (Expect(TokenType.Semicolon))
                {
                    Consume();
                } else
                {
                    Console.WriteLine($"Compiler Error: Expected ';', got {Peek().value}.");

                    return null;
                }
            } else if (Expect(TokenType.RoundBracket))
            {
                
            } else
            {
                Console.WriteLine($"Compiler Error: Undefined statement '{Peek().value}'.");

                return null;
            }
        } else
        {
            Console.WriteLine($"Compiler Error: Expected '(', got {Peek().value}.");

            return null;
        }

        return printNode;
    }
}