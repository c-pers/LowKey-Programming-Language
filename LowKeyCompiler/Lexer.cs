using System;

public class Lexer
{
    // Initializing read only source code and position index (analyzing char-by-char)
    private readonly string _source;
    private int _position = 0;

    // Default constructor
    public Lexer(string source)
    {
        _source = source;
    }

    // Checks if we reached the end of the source file
    private bool IsAtEnd() => _position >= _source.Length;

    // Checks the current char in _position. Returns an empty character if we reached the end
    private char Peek()
    {
        if (IsAtEnd()) return '\0';

        return _source[_position];
    }

    // Peeks the current character and increments the _position variable
    private char Advance()
    {
        char currentChar = Peek();

        _position++;

        return currentChar;
    }

    // Method to add the tokens to the list of tokens
    public List<Token> Tokenize()
    {
        var tokens = new List<Token>();

        // Repeat until the end of the file isn't reached
        while (!IsAtEnd())
        {
            char c = Peek();

            // If its a white space we advance and skip
            if (char.IsWhiteSpace(c))
            {
                Advance();

                continue;
            }

            // If its a letter we use ReadWord() to get all the adjacent letters or digits
            if (char.IsLetter(c))
            {
                string word = ReadWord();

                // Checks the token type
                // It requires a keyword or an identifier
                TokenType type = KeywordOrIdentifier(word);

                // Adds the new token to the list
                tokens.Add(new Token(type, word));

                continue;
            }

            // If its a digit we use ReadNumber(). Works similar to ReadWord()
            if (char.IsDigit(c))
            {
                string number = ReadNumber();

                // Adds the new token to the list
                tokens.Add(new Token(TokenType.Number, number));

                continue;
            }

            // In any other case we increment the _position variable
            Advance();
            
            // Adds the token to the list with a TEMPORARY operator type
            // (happens for ;, =, etc)
            tokens.Add(new Token(TokenType.Operator, c.ToString()));
        }

        // Once the while is ended we add a token of type EOF to the list
        tokens.Add(new Token(TokenType.EOF, ""));

        // Returns the list
        return tokens;
    }

    // Checks if the given text is a lk keyword or an identifier (such as variable names)
    private TokenType KeywordOrIdentifier(string text)
    {
        return text switch
        {
            "int" or "string" or "if" or "else" or "end" or "do" or "while" => TokenType.Keyword,
            _ => TokenType.Identifier
        };
    }

    // Method to get adjacent letters or digit
    private string ReadWord()
    {
        int start = _position;

        while (!IsAtEnd() && char.IsLetterOrDigit(Peek()))
        {
            Advance();
        }

        return _source.Substring(start, _position - start);
    }

    // Method to get adjacent digits
    private string ReadNumber()
    {
        int start = _position;

        while (!IsAtEnd() && char.IsDigit(Peek()))
        {
            Advance();
        }

        return _source.Substring(start, _position - start);
    }
}